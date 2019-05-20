using System.Collections;
using Innoactive.Hub.Training.Utils.Serialization;
using UnityEngine;

namespace Innoactive.Hub.Training.Template
{
    public class TrainingLoader : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Text asset with a saved training course.")]
        private TextAsset serializedTrainingCourse;

        private IEnumerator Start()
        {
            // Skip the first two frames to give VRTK time to initialize.
            yield return null;
            yield return null;

            // Load a training from the text asset.
            ITraining training = JsonTrainingSerializer.Deserialize(serializedTrainingCourse.text);

            // Start the training execution.
            training.Activate();
        }
    }
}