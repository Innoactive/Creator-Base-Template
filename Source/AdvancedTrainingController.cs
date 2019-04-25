using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common.Logging;
using Innoactive.Hub.Threading;
using Innoactive.Hub.Training.Utils.Serialization;
using Innoactive.Hub.TextToSpeech;
using Innoactive.Hub.Training.Configuration;
using Innoactive.Hub.Training.TextToSpeech;
using Innoactive.Hub.Training.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using LogManager = Innoactive.Hub.Logging.LogManager;

namespace Innoactive.Hub.Training.Template
{
    /// <summary>
    /// Controller class for an example of a custom training overlay with audio and localization.
    /// </summary>
    public class AdvancedTrainingController : MonoBehaviour
    {
        private static readonly ILog logger = LogManager.GetLogger<AdvancedTrainingController>();

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
        [Tooltip("The folder and file name (without the extension .json) of the serialized training in the 'Training' directory of the 'StreamingAssets' directory which should be loaded.")]
        [SerializeField]
        private string trainingName;

        [Tooltip("The two-letter ISO language code (e.g. \"EN\") of the fallback language which is used by the text to speech engine if no valid localization file is found.")]
        [SerializeField]
        private string fallbackLanguage = "EN";

        private List<String> localizationFileNames;

        private string selectedLanguage;

        private ITraining training;

        // Called once when object is created.
        private void Awake()
        {
            // Create new audio source and make it the default audio player.
            //TrainingConfiguration.Instance.InstructionPlayer = gameObject.AddComponent<AudioSource>();

            // Get the current system language as default language.
            selectedLanguage = LocalizationUtils.GetSystemLanguageAsTwoLetterIsoCode();

            // Check if the fallback language is a valid language.
            fallbackLanguage = fallbackLanguage.Trim();
            string validFallbackLanguage;
            if (fallbackLanguage.TryConvertToTwoLetterIsoCode(out validFallbackLanguage))
            {
                fallbackLanguage = validFallbackLanguage;
            }
            // If not, use "EN" instead.
            else
            {
                logger.WarnFormat("'{0}' is no valid language. Changed fallback language to 'EN'.", fallbackLanguage);
                fallbackLanguage = "EN";
            }

            // Get all the available localization files for the selected training.
            localizationFileNames = FetchAvailableLocalizationsForTraining();

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
            TrainingConfiguration.Definition.TextToSpeechConfig = ttsConfig;

            // Load the localization file of the current selected language.
            LoadLocalizationForTraining();

            // Load training from a file. That will synthesize an audio for the training instructions, too.
            training = LoadTraining();
        }

        private List<string> FetchAvailableLocalizationsForTraining()
        {
            // Get the directory of all localization files of the selected training.
            // It should be in the '[YOUR_PROJECT_ROOT_FOLDER]/StreamingAssets/Training/[TRAINING_NAME]' folder.
            string pathToLocalizations = string.Format("{0}/Training/{1}/Localization/", Application.streamingAssetsPath, trainingName);

            // Save all existing localization files in a list.
            List<string> availableLocalizations = new List<string>();

            // Check if the "Localization" directory really exists.
            if (Directory.Exists(pathToLocalizations))
            {
                // Parse the names without extension (.json) of all localization files.
                // The name should be a valid two-letter ISO code (which also can be three letters long).
                availableLocalizations = Directory.GetFiles(pathToLocalizations, "*.json").ToList()
                    .ConvertAll(Path.GetFileNameWithoutExtension)
                    .Where(f => f.Length <= 3 && f.TryConvertToTwoLetterIsoCode(out f))
                    .ToList();

                // If there are no valid files, log a warning.
                if (availableLocalizations.Count == 0)
                {
                    logger.WarnFormat("There are no valid localization files in '{0}'. Make sure that the JSON files are named after their languages in the two-letter ISO code format.", pathToLocalizations);
                }
            }
            else
            {
                // If there is no "Localization" directory, log a warning.
                logger.WarnFormat("The localization path '{0}' does not exist. No localization files can be loaded.", pathToLocalizations);
            }

            // Return the list of all available valid localizations.
            return availableLocalizations;
        }

        private bool LoadLocalizationForTraining()
        {
            // Find the correct file name of the current selected language.
            string language = localizationFileNames.Find(f => string.Equals(f, selectedLanguage, StringComparison.CurrentCultureIgnoreCase));

            // Get the path to the file.
            // It should be in the '[YOUR_PROJECT_ROOT_FOLDER]/StreamingAssets/Training/[TRAINING_NAME]/Localization' folder.
            string pathToLocalization = string.Format("{0}/Training/{1}/Localization/{2}.json", Application.streamingAssetsPath, trainingName, language);

            // Check if the file really exists and load it.
            if (File.Exists(pathToLocalization))
            {
                Localization.LoadLocalization(pathToLocalization);
                return true;
            }

            // Log a warning if no language file was found.
            logger.WarnFormat("No language file for language '{0}' found for training {1} at '{2}'.", selectedLanguage, trainingName, pathToLocalization);
            return false;
        }

        private ITraining LoadTraining()
        {
            // Get the path to the file.
            // It should be in the '[YOUR_PROJECT_ROOT_FOLDER]/StreamingAssets/Training/[TRAINING_NAME]' folder.
            string pathToTraining = string.Format("{0}/Training/{1}/{1}.json", Application.streamingAssetsPath, trainingName);

            // Check if the file really exists and return the deserialized file text.
            if (File.Exists(pathToTraining))
            {
                return JsonTrainingSerializer.Deserialize(File.ReadAllText(pathToTraining));
            }

            // Otherwise, throw an exception.
            throw new ArgumentException("The file or the path to the file does not exist. Thus, no serialized training can be loaded.", pathToTraining);
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
                // Disable the mode picker as it is not allowed to change the mode during the training's execution.
                modePicker.interactable = false;
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

                TrainingConfiguration.Definition.InstructionPlayer.mute = newValue == false;
            });
        }

        private void SetupLanguagePicker()
        {
            // List of the options to display to user.
            List<string> supportedLanguages = new List<string>();

            // Add each language in capital letters to the list of supported languages.
            foreach (string file in localizationFileNames)
            {
                supportedLanguages.Add(file.ToUpper());
            }

            // Setup the dropdown menu.
            languagePicker.AddOptions(supportedLanguages);

            // Set the picker value to the current selected language.
            int languageValue = supportedLanguages.IndexOf(selectedLanguage.ToUpper());

            if (languageValue > -1)
            {
                languagePicker.value = languageValue;
            }
            // If the selected language (system language when starting the scene) has no valid localization file.
            else
            {
                // Either choose the first language of all languages as selected language.
                if (supportedLanguages.Count > 0)
                {
                    selectedLanguage = supportedLanguages[languagePicker.value];
                }
                // Or use the fallback language, if there is no valid localization file at all.
                else
                {
                    selectedLanguage = fallbackLanguage;
                    // Add the fallback language as option of the dropdown menu. Otherwise, the picker would be empty.
                    languagePicker.AddOptions(new List<string>(){ fallbackLanguage.ToUpper() });
                }
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

            // Add each mode name to the list of available modes.
            foreach (IMode mode in TrainingConfiguration.Definition.AvailableModes)
            {
                availableModes.Add(mode.Name);
            }

            // Setup the dropdown menu.
            modePicker.AddOptions(availableModes);

            // Set the picker value to the current selected mode.
            modePicker.value = TrainingConfiguration.Definition.GetCurrentModeIndex();

            // When the selected mode is changed, setup a training from scratch.
            modePicker.onValueChanged.AddListener(itemIndex =>
            {
                // Set the mode based on the user selection.
                TrainingConfiguration.Definition.SetMode(itemIndex);
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
            training.ActivationStarted += (sender, args) => trainingStateIndicator.enabled = true;
            // When training is completed, hide it again.
            training.Activated += (sender, args) => trainingStateIndicator.enabled = false;
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
