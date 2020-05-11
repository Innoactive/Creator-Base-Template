using UnityEngine;
using System.Collections;
using Innoactive.Creator.Core;
using Innoactive.Creator.Core.Configuration;

namespace Innoactive.Creator.BasicTemplate
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
            string coursePath = RuntimeConfigurator.Instance.GetSelectedCourse();
            ICourse trainingCourse = RuntimeConfigurator.Configuration.LoadCourse(coursePath);

            // Start the training execution.
            CourseRunner.Initialize(trainingCourse);
            CourseRunner.Run();
        }
    }
}
