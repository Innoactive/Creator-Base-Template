using System.Collections;
using Innoactive.Hub.Training.Configuration;
using UnityEngine;

namespace Innoactive.Hub.Training.Template
{
    /// <summary>
    /// Loads and starts the training course currently selected in the '[TRAINING_CONFIGURATION]' gameObject.
    /// </summary>
    public class TrainingCourseLoader : MonoBehaviour
    {
       private IEnumerator Start()
        {
            // Skip the first two frames to give VRTK time to initialize.
            yield return null;
            yield return null;

            // Load the currently selected training course.
            ICourse trainingCourse = RuntimeConfigurator.Configuration.LoadCourse();

            // Start the training execution.
            TrainingRunner.Initialize(trainingCourse);
            TrainingRunner.Run();
        }
    }
}
