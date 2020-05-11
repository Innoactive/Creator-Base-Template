﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Innoactive.Creator.Core;
using Innoactive.Creator.Unity;
using Innoactive.Creator.TextToSpeech;
using Innoactive.Creator.Core.Configuration;
using Innoactive.Creator.Core.Configuration.Modes;
using Innoactive.Creator.Core.Internationalization;

namespace Innoactive.Creator.BasicTemplate
{
    /// <summary>
    /// Controller class for an example of a custom training overlay with audio and localization.
    /// </summary>
    public class AdvancedCourseController : MonoBehaviour
    {
        #region UI elements
        [Tooltip("Chapter picker dropdown.")]
        [SerializeField]
        private Dropdown chapterPicker;

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

        [Tooltip("Step picker dropdown.")]
        [SerializeField]
        private Dropdown skipStepPicker;

        [Tooltip("Button that resets the scene to its initial state.")]
        [SerializeField]
        private Button resetSceneButton;

        [Tooltip("Toggle that turns training audio on or off.")]
        [SerializeField]
        private Toggle soundToggle;
        
        [Tooltip("Image that shows the sound icon.")]
        [SerializeField]
        private Image soundImage;

        [Tooltip("Icon that indicates that sound is enabled.")]
        [SerializeField]
        private Sprite soundOnImage;

        [Tooltip("Icon that indicates that sound is disabled.")]
        [SerializeField]
        private Sprite soundOffImage;

        [Tooltip("Language picker dropdown.")]
        [SerializeField]
        private Dropdown languagePicker;

        [Tooltip("Mode picker dropdown.")]
        [SerializeField]
        private Dropdown modePicker;
        #endregion

        [Tooltip("The two-letter ISO language code (e.g. \"EN\") of the fallback language which is used by the text to speech engine if no valid localization file is found.")]
        [SerializeField]
        private string fallbackLanguage = "EN";

        private List<string> localizationFileNames;

        private string selectedLanguage;
        private FieldInfo skipStepPickerEditorValueField;

        private IStep displayedStep;
        private ICourse trainingCourse;
        private IChapter lastDisplayedChapter;

        private void Awake()
        {
            try
            {
                // Load training course from a file.
                string coursePath = RuntimeConfigurator.Instance.GetSelectedTrainingCourse();
                trainingCourse = RuntimeConfigurator.Configuration.LoadCourse(coursePath);
            }
            catch (Exception exception)
            {
                Debug.LogError($"{exception.GetType().Name}, {exception.Message}\n{exception.StackTrace}", RuntimeConfigurator.Instance.gameObject);
            }

            // Get the current system language as default language.
            selectedLanguage = LocalizationUtils.GetSystemLanguageAsTwoLetterIsoCode();

            // Check if the fallback language is a valid language.
            fallbackLanguage = fallbackLanguage.Trim();
            
            if (fallbackLanguage.TryConvertToTwoLetterIsoCode(out string validFallbackLanguage))
            {
                fallbackLanguage = validFallbackLanguage;
            }
            // If not, use "EN" instead.
            else
            {
                Debug.LogWarningFormat("'{0}' is no valid language. Changed fallback language to 'EN'.", fallbackLanguage);
                fallbackLanguage = "EN";
            }

            // Get all the available localization files for the selected training.
            localizationFileNames = FetchAvailableLocalizationsForTraining();

            // Setup UI controls.
            SetupChapterPicker();
            SetupStepInfoToggle();
            SetupStartTrainingButton();
            SetupSkipStepPicker();
            SetupResetSceneButton();
            SetupSoundToggle();
            SetupLanguagePicker();
            SetupModePicker();

            // Load the training and localize it to the selected language.
            SetupTraining();
            
            // Update the UI.
            SetupTrainingDependantUI();
        }

        private void Update()
        {
            IChapter currentChapter = CourseRunner.Current == null ? null : CourseRunner.Current.Data.Current;
            IStep currentStep = currentChapter?.Data.Current;

            if (currentChapter != lastDisplayedChapter)
            {
                lastDisplayedChapter = currentChapter;
                UpdateDisplayedChapter(currentChapter);
            }

            if (currentStep != displayedStep)
            {
                displayedStep = currentStep;
                UpdateDisplayedStep(currentStep);
            }
        }

        private void UpdateDisplayedStep(IStep step)
        {
            if (step == null)
            {
                // If there is no next step, clear the info text.
                stepInfoText.text = string.Empty;
                stepName.text = string.Empty;
            }
            else
            {
                // Else, assign the description of the new step.
                stepInfoText.text = step.Data.Description;
                stepName.text = step.Data.Name;
            }

            SetupSkipStepPickerOptions();
        }

        private void UpdateDisplayedChapter(IChapter chapter)
        {
                // Get a collection of available chapters.
                IList<IChapter> chapters = CourseRunner.Current == null ? new List<IChapter>() : CourseRunner.Current.Data.Chapters.ToList();

                // Skip all finished chapters.
                int startingIndex = chapter == null ? 0 : chapters.IndexOf(chapter);

                // Show the rest.
                PopulateChapterPickerOptions(startingIndex);
        }

        private void SetupTraining()
        {
            // You can define which TTS engine is used through TTS config.
            TextToSpeechConfiguration ttsConfiguration = RuntimeConfigurator.Configuration.GetTextToSpeechConfiguration();
            
            // Define which TTS provider is used.
            ttsConfiguration.Provider = nameof(MicrosoftSapiTextToSpeechProvider);
            
            // The acceptable values for the Voice and the Language differ from TTS provider to provider.
            // Microsoft SAPI TTS provider takes either "Male" or "Female" value as a voice.
            ttsConfiguration.Voice = "Female";
            
            // Microsoft SAPI TTS provider takes either natural language name, or two-letter ISO language code.
            ttsConfiguration.Language = selectedLanguage;

            // If TTS config overload is set, it is used instead the config that is located at `[YOUR_PROJECT_ROOT_FOLDER]/Config/text-to-speech-config.json`.
            RuntimeConfigurator.Configuration.SetTextToSpeechConfiguration(ttsConfiguration);

            // Load the localization file of the current selected language.
            LoadLocalizationForTraining();
            
            // Initializes the training course. That will synthesize an audio for the training instructions, too.
            CourseRunner.Initialize(trainingCourse);
        }

        private List<string> FetchAvailableLocalizationsForTraining()
        {
            // Get the directory of all localization files of the selected training.
            // It should be in the '[YOUR_PROJECT_ROOT_FOLDER]/StreamingAssets/Training/[TRAINING_NAME]' folder.
            string pathToCourse = Path.GetDirectoryName(Path.Combine(Application.streamingAssetsPath, RuntimeConfigurator.Instance.GetSelectedCourse()));
            string pathToLocalizations = $"{pathToCourse}/Localization/".Replace('/', Path.DirectorySeparatorChar);

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
                    Debug.LogWarningFormat("There are no valid localization files in '{0}'. Make sure that the JSON files are named after their languages in the two-letter ISO code format.", pathToLocalizations);
                }
            }
            else
            {
                // If there is no "Localization" directory, log a warning.
                Debug.LogWarningFormat("The localization path '{0}' does not exist. No localization files can be loaded.", pathToLocalizations);
            }

            // Return the list of all available valid localizations.
            return availableLocalizations;
        }

        private void LoadLocalizationForTraining()
        {
            // Find the correct file name of the current selected language.
            string language = localizationFileNames.Find(f => string.Equals(f, selectedLanguage, StringComparison.CurrentCultureIgnoreCase));

            // Get the path to the file.
            // It should be in the '[YOUR_PROJECT_ROOT_FOLDER]/StreamingAssets/Training/[TRAINING_NAME]/Localization' folder.
            string pathToCourse = Path.GetDirectoryName(Path.Combine(Application.streamingAssetsPath, RuntimeConfigurator.Instance.GetSelectedCourse()));
            string pathToLocalization = $"{pathToCourse}/Localization/{language}.json".Replace('/', Path.DirectorySeparatorChar);

            // Check if the file really exists and load it.
            if (File.Exists(pathToLocalization))
            {
                Localization.LoadLocalization(pathToLocalization);
                return;
            }

            // Log a warning if no language file was found.
            Debug.LogWarningFormat("No language file for language '{0}' found for training at '{1}'.", selectedLanguage, pathToCourse);
        }

        private void FastForwardChapters(int numberOfChapters)
        {
            // Skip if no chapters have to be fast-forwarded.
            if (numberOfChapters == 0)
            {
                return;
            }

            CourseRunner.SkipChapters(numberOfChapters);
        }

        #region Setup UI
        private void SetupChapterPicker()
        {
            // When selected chapter has changed,
            chapterPicker.onValueChanged.AddListener(index =>
            {
                if (CourseRunner.IsRunning == false)
                {
                    return;
                }

                // If the training hasn't started it, ignore it. We will use this value when the training starts.
                if (CourseRunner.Current.LifeCycle.Stage == Stage.Inactive)
                {
                    return;
                }

                // Otherwise, fast forward the chapters until the selected is active.
                FastForwardChapters(index);
            });
        }

        private void SetupStepInfoToggle()
        {
            // When info toggle is pressed,
            stepInfoToggle.onValueChanged.AddListener(newValue =>
            {
                if (string.IsNullOrEmpty(stepInfoText.text))
                {
                    // Show or hide description of the step.
                    stepInfoBackground.enabled = false;
                    stepInfoText.enabled = false;
                    return;
                }
                
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
                if (CourseRunner.Current == null)
                {
                    Debug.LogError("No training course is selected.", RuntimeConfigurator.Instance.gameObject);
                    return;
                }
                
                // Subscribe to the "stage changed" event of the current training in order to change the skip step button to the start button after finishing the training.
                CourseRunner.Current.LifeCycle.StageChanged += (sender, args) =>
                {
                    if (args.Stage == Stage.Inactive)
                    {
                        skipStepPicker.gameObject.SetActive(false);
                        startTrainingButton.gameObject.SetActive(true);
                    }
                };

                //Skip all chapters before selected.
                FastForwardChapters(chapterPicker.value);

                // Start the training
                CourseRunner.Run();
                
                // Show the skip step button instead of the start button.
                skipStepPicker.gameObject.SetActive(true);
                startTrainingButton.gameObject.SetActive(false);

                // Disable button as you have to reset scene before starting the training again.
                startTrainingButton.interactable = false;
                // Disable the language picker as it is not allowed to change the language during the training's execution.
                languagePicker.interactable = false;
            });
        }

        private void SetupSkipStepPicker()
        {
            // Dropdown.onValueChanged won't be call if Dropdown.value is equal to the selected value. 
            // Dropdown.value can't be less than 0 or grater than Dropdown.options.Count -1.
            // This causes the dropdown to never call onValueChanged in cases when there is only 1 transition.
            // By setting the value to be out of range from the "editor" instead than from Dropdown.value we ensure that Dropdown.onValueChanged is always called.
            skipStepPickerEditorValueField = skipStepPicker.GetType().GetField("m_Value", BindingFlags.NonPublic | BindingFlags.Instance);
            
            // When a target step was chosen,
            skipStepPicker.onValueChanged.AddListener(index =>
            {
                // If there's an active step and it's not the last step,
                if (displayedStep != null && index < displayedStep.Data.Transitions.Data.Transitions.Count && displayedStep.LifeCycle.Stage != Stage.Inactive)
                {
                    CourseRunner.SkipStep(displayedStep.Data.Transitions.Data.Transitions[index]);
                }
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
            soundToggle.onValueChanged.AddListener(isSoundOn =>
            {
                // Set active image for sound.
                soundImage.sprite = isSoundOn ? soundOnImage : soundOffImage;

                // Mute the instruction audio.
                RuntimeConfigurator.Configuration.InstructionPlayer.mute = isSoundOn == false;
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
                    languagePicker.AddOptions(new List<string>() { fallbackLanguage.ToUpper() });
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
                SetupTrainingDependantUI();
            });
            
            // If there is only one option, the dropdown is currently disabled.
            languagePicker.enabled = supportedLanguages.Count > 1;
        }

        private void SetupModePicker()
        {
            // List of the options to display to user.
            List<string> availableModes = new List<string>();

            // Add each mode name to the list of available modes.
            foreach (IMode mode in RuntimeConfigurator.Configuration.Modes.AvailableModes)
            {
                availableModes.Add(mode.Name);
            }
            
            // Setup the dropdown menu.
            modePicker.AddOptions(availableModes);
            
            // Set the picker value to the current selected mode.
            modePicker.value = RuntimeConfigurator.Configuration.Modes.CurrentModeIndex;
            
            // If there is only one option, the dropdown is currently disabled.
            modePicker.enabled = availableModes.Count > 1;
            
            // When the selected mode is changed,
            modePicker.onValueChanged.AddListener(itemIndex =>
            {
                // Set the mode based on the user selection.
                RuntimeConfigurator.Configuration.Modes.SetMode(itemIndex);
            });
        }
        #endregion

        #region Setup training-dependant UI
        private void SetupTrainingDependantUI()
        {
            SetupChapterPickerOptions();
            SetupTrainingIndicator();
        }

        private void SetupChapterPickerOptions()
        {
            // Show all chapters of the training.
            PopulateChapterPickerOptions(0);
        }

        private void PopulateChapterPickerOptions(int startingIndex)
        {
            // Get a collection of available chapters.
            IList<IChapter> chapters = CourseRunner.Current?.Data.Chapters;

            if (chapters != null)
            {
                // Skip finished chapters and convert the rest to a list of chapter names.
                List<string> dropdownOptions = new List<string>();
                
                for (int i = startingIndex; i < chapters.Count; i++)
                {
                    dropdownOptions.Add(chapters[i].Data.Name);
                }

                // Reset the chapter picker.
                chapterPicker.ClearOptions();

                // Populate it with new options.
                chapterPicker.AddOptions(dropdownOptions);

                // Reset the selected value
                chapterPicker.value = 0;

                // If there is only one option, the dropdown is currently disabled.
                chapterPicker.enabled = dropdownOptions.Count > 1;
            }
        }

        private void SetupSkipStepPickerOptions()
        {
            // Reset the skip step picker.
            skipStepPicker.ClearOptions();

            if (displayedStep == null)
            {
                return;
            }

            // Get a collection of available transitions (one per target step).
            IList<ITransition> transitions = displayedStep.Data.Transitions.Data.Transitions.ToList();

            // Create a list with all dropdown option names.
            List<string> dropdownOptions = new List<string>();

            if (transitions.Count > 0)
            {
                // Convert the transitions to a list of target step names and use them as dropdown options.
                // null as target step means "end of chapter".
                dropdownOptions = transitions.Select(transition => (transition.Data.TargetStep != null) ? transition.Data.TargetStep.Data.Name : "End of the Chapter").ToList();
            }

            // Populate it with new options.
            skipStepPicker.AddOptions(dropdownOptions);
            skipStepPickerEditorValueField?.SetValue(skipStepPicker, dropdownOptions.Count);
        }

        private void SetupTrainingIndicator()
        {
            if (CourseRunner.Current == null)
            {
                return;
            }
            
            CourseRunner.Current.LifeCycle.StageChanged += (sender, args) =>
            {
                if (args.Stage == Stage.Activating)
                {
                    // Show the indicator when the training is started.
                    trainingStateIndicator.enabled = true;
                }

                if (args.Stage == Stage.Active)
                {
                    // When the training is completed, hide it again.
                    trainingStateIndicator.enabled = false;
                }
            };
        }
        #endregion
    }
}
