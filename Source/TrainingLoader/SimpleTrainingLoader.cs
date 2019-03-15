using System.Collections;
using Innoactive.Hub.Training.Utils.Serialization;
using UnityEngine;

namespace Innoactive.Hub.Training.Template
{
	public class SimpleTrainingLoader : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("Text asset with saved training.")]
		private TextAsset serializedTraining;

		private IEnumerator Start()
		{
			yield return null;
			yield return null;

			// Load a training from the text asset
			ITraining training = JsonTrainingSerializer.Deserialize(serializedTraining.text);

			// Start the training execution
			training.Activate();
		}
	}
}
