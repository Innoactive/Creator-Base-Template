using System.Collections.Generic;
using Innoactive.Hub.Threading;
using Innoactive.Hub.Training.Utils.Serialization;
using Innoactive.Hub.TextToSpeech;
using Innoactive.Hub.Training.Configuration;
using Innoactive.Hub.Training.TextToSpeech;
using Innoactive.Hub.Training.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Innoactive.Hub.Training.Template
{
    /// <summary>
    /// Controller class for an example of a custom training overlay with audio and localization.
    /// </summary>
    public class AdvancedTrainingController : MonoBehaviour
    {
        #region UI elements
        [Tooltip("The image next to a step name which is visible when a training is running.")]
        [SerializeField]
        private Image trainingStateIndicator;

        [Tooltip("Name of the step that is currently executed.")]
        [SerializeField]
        private Text stepName;

        [Tooltip("Button that shows additional information about the step.")]
        [SerializeField]
        private Toggle stepInfoToggle;

        [Tooltip("Background for additional step information.")]
        [SerializeField]
        private Image stepInfoBackground;

        [Tooltip("Short description of the text which is visible when Info Toggle is toggled on.")]
        [SerializeField]
        private Text stepInfoText;

        [Tooltip("Button that starts execution of the training.")]
        [SerializeField]
        private Button startTrainingButton;

        [Tooltip("Button that resets the scene to its initial state.")]
        [SerializeField]
        private Button resetSceneButton;

        [Tooltip("Toggle that turns training audio on or off.")]
        [SerializeField]
        private Toggle soundToggle;

        [Tooltip("Icon that indicates that sound is enabled.")]
        [SerializeField]
        private Image soundOnImage;

        [Tooltip("Icon that indicates that sound is disabled.")]
        [SerializeField]
        private Image soundOffImage;

        [Tooltip("Language picker dropdown.")]
        [SerializeField]
        private Dropdown languagePicker;
        
        [Tooltip("Mode picker dropdown.")]
        [SerializeField]
        private Dropdown modePicker;
        #endregion

        [Space]
        [Tooltip("Text asset with a serialized training.")]
        [SerializeField]
        private TextAsset serializedTraining;

        [Tooltip("Json files with localizations for different languages.")]
        [SerializeField]
        private TextAsset[] localizationFiles;

        private string selectedLanguage;

        private ITraining training;

        // Called once when object is created.
        private void Awake()
        {
            // Create new audio source and make it the default audio player.
            //TrainingConfiguration.Instance.InstructionPlayer = gameObject.AddComponent<AudioSource>();

            // Get the current system language as default language.
            selectedLanguage = LocalizationUtils.GetSystemLanguageAsTwoLetterIsoCode();

            // Setup UI controls.
            SetupStepInfoToggle();
            SetupStartTrainingButton();
            SetupResetSceneButton();
            SetupSoundToggle();
            SetupLanguagePicker();
            SetupModePicker();

            // Load the training and localize it to the selected language.
            SetupTraining();
            // Update the UI.
            SetupTrainingDependantUi();
        }

        private void SetupTraining()
        {
            // You can define which TTS engine is used through TTS config.
            TextToSpeechConfig ttsConfig = new TextToSpeechConfig()
            {
                // Define which TTS provider is used.
                Provider = typeof(MicrosoftSapiTextToSpeechProvider).Name,

                // The acceptable values for the Voice and the Language differ from TTS provider to provider.
                // Microsoft SAPI TTS provider takes either "Male" or "Female" value as a voice.
                Voice = "Female",

                // Microsoft SAPI TTS provider takes either natural language name, or two-letter ISO language code.
                Language = selectedLanguage
            };

            // If TTS config overload is set, it is used instead the config that is located at `[YOUR_PROJECT_ROOT_FOLDER]/Config/text-to-speech-config.json`.
            TrainingConfiguration.Instance.TextToSpeechConfig = ttsConfig;

            // Determine which localization is used.
            string serializedLocalization;
            // If the localization was found,
            if (LocalizationUtils.TryFindLocalizationForLanguage(selectedLanguage, localizationFiles, out serializedLocalization))
            {
                // Parse the json text as a dictionary of strings and pass it to the localization system.
                Localization.entries = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(serializedLocalization);
            }

            // Load training from a file. That will synthesize an audio for the training instructions, too.
            training = JsonTrainingSerializer.Deserialize(serializedTraining.text);
        }

        #region Setup UI
        private void SetupStepInfoToggle()
        {
            // When info toggle is pressed,
            stepInfoToggle.onValueChanged.AddListener(newValue =>
            {
                // Show or hide description of the step.
                stepInfoBackground.enabled = newValue;
                stepInfoText.enabled = newValue;
            });
        }

        private void SetupStartTrainingButton()
        {
            // When user clicks on Start Training button,
            startTrainingButton.onClick.AddListener(() =>
            {
                // Start the training
                training.Activate();

                // Disable button as you have to reset scene before starting the training again.
                startTrainingButton.interactable = false;
                // Disable the language picker as it is not allowed to change the language during the training's execution.
                languagePicker.interactable = false;
            });
        }

        private void SetupResetSceneButton()
        {
            // When user clicks on Reset Scene button,
            resetSceneButton.onClick.AddListener(() =>
            {
                // Stop all coroutines. The Training SDK is in closed beta stage, and we can't properly interrupt a training yet.
                // For example, consider the Move Object behavior: it changes position of an object over time.
                // Even if you would reload the scene, it would still be moving that object, which will lead to unwanted result.
                CoroutineDispatcher.Instance.StopAllCoroutines();

                // Reload current scene.
                SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
            });
        }

        private void SetupSoundToggle()
        {
            // When sound toggle is clicked,
            soundToggle.onValueChanged.AddListener(newValue =>
            {
                // Show one icon and hide another.
                soundOnImage.enabled = newValue;
                soundOffImage.enabled = newValue == false;

                TrainingConfiguration.Instance.InstructionPlayer.mute = newValue == false;
            });
        }

        private void SetupLanguagePicker()
        {
            // List of the options to display to user.
            List<string> supportedLanguages = new List<string>();
            foreach (TextAsset file in localizationFiles)
            {
                string twoLetterIsoCode;

                // Try to convert filename to two-letter ISO code.
                if (file.name.TryConvertToTwoLetterIsoCode(out twoLetterIsoCode) == false)
                {
                    // If file's name can't be converted to ISO code, skip it.
                    continue;
                }

                // Capitalize it so it would look prettier.
                twoLetterIsoCode = twoLetterIsoCode.ToUpper();

                // Add it to the list of supported languages.
                supportedLanguages.Add(twoLetterIsoCode);
            }

            // Setup the dropdown menu.
            languagePicker.AddOptions(supportedLanguages);

            // Set the picker value to the current selected language.
            int languageValue = supportedLanguages.IndexOf(selectedLanguage.ToUpper());

            if (languageValue > -1)
            {
                languagePicker.value = languageValue;
            }

            // When the selected language is changed, setup a training from scratch.
            languagePicker.onValueChanged.AddListener(itemIndex =>
            {
                // Set the supported language based on the user selection.
                selectedLanguage = supportedLanguages[itemIndex];
                // Load the training and localize it to the selected language.
                SetupTraining();
                // Update the UI.
                SetupTrainingDependantUi();
            });
        }

        private void SetupModePicker()
        {
            // List of the options to display to user.
            List<string> availableModes = new List<string>();
            
            foreach (IMode mode in TrainingConfiguration.Instance.AvailableModes)
            {
                // Add it to the list of available modes.
                availableModes.Add(mode.Name);
            }
            
            // Setup the dropdown menu.
            modePicker.AddOptions(availableModes);
            
            // Set the picker value to the current selected mode.
            modePicker.value = TrainingConfiguration.Instance.GetCurrentModeIndex();

            // When the selected mode is changed, setup a training from scratch.
            modePicker.onValueChanged.AddListener(itemIndex =>
            {
                // Set the mode based on the user selection.
                TrainingConfiguration.Instance.SetMode(itemIndex);
                // Load the training.
                SetupTraining();
                // Update the UI.
                SetupTrainingDependantUi();
            });
        }
        #endregion

        #region Setup training-dependant UI
        private void SetupTrainingDependantUi()
        {
            SetupTrainingIndicator();
            SetupStepName();
            SetupStepInfo();
        }

        private void SetupTrainingIndicator()
        {
            // When training is started show the indicator.
            training.TrainingStarted += (sender, args) => trainingStateIndicator.enabled = true;
            // When training is completed, hide it again.
            training.TrainingCompleted += (sender, args) => trainingStateIndicator.enabled = false;
        }

        private void SetupStepName()
        {
            // When current step has changed,
            training.ActiveStepChanged += (sender, args) =>
            {
                if (args.CurrentStep == null)
                {
                    // If there is no next step, clear the label.
                    stepName.text = "";
                }
                else
                {
                    // Else, assign the name of the new step.
                    stepName.text = args.CurrentStep.Name;
                }
            };
        }

        private void SetupStepInfo()
        {
            // When current step has changed,
            training.ActiveStepChanged += (sender, args) =>
            {
                if (args.CurrentStep == null)
                {
                    // If there is no next step, clear the info text.
                    stepInfoText.text = "";
                }
                else
                {
                    // Else, assign the description of the new step.
                    stepInfoText.text = args.CurrentStep.Description;
                }
            };
        }
        #endregion
    }
}
