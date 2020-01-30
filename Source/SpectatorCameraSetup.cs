using System;
using Innoactive.Hub.SDK.Models;
using Innoactive.Hub.Training.Configuration;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VRTK;

namespace Innoactive.Hub.PlayerSetup
{
    /// <summary>
    /// Does the complete setup of the player. This includes disabling the headset player if
    /// chosen in the config, spawning the spectator cam if activated in config and
    /// configuring the cameras to the scene specific needs.
    /// </summary>
    public class SpectatorCameraSetup : MonoBehaviour
    {
        private static readonly Common.Logging.ILog logger = Logging.LogManager.GetLogger<SpectatorCameraSetup>();

        [Tooltip("This prefab will be used if set. Otherwise, the default Spectator Cam is used.")]
        [SerializeField]
        protected GameObject spectatorCamPrefabOverload;

        [Tooltip("The font used in the spectator view.")]
        [SerializeField]
        protected Font font;
        
        [Tooltip("Size of the font used")]
        [SerializeField]
        protected int fontSize = 30;

        private GameObject trainee;
        
        private GameObject currentSpectatorInstance = null;

        protected virtual void Start()
        {
            InstantiateSpectator();
        }
        
        private void InstantiateSpectator()
        {
            if (currentSpectatorInstance != null)
            {
                Destroy(currentSpectatorInstance);
            }
            GameObject spectatorPrefab = spectatorCamPrefabOverload == null ? Resources.Load<GameObject>("Spectator Camera") : spectatorCamPrefabOverload;
            currentSpectatorInstance = Instantiate(spectatorPrefab, transform.position, transform.rotation);
            SetFont();
        }

        private void SetFont()
        {
            foreach (Text text in currentSpectatorInstance.GetComponentsInChildren<Text>(true))
            {
                text.font = font;
                text.fontSize = fontSize;
            }
        }

        protected void LateUpdate()
        {
            UpdateCameraPositionAndRotation();
        }

        /// <summary>
        /// Set the position and rotation depending on the passed time.
        /// </summary>
        protected virtual void UpdateCameraPositionAndRotation()
        {
            if (trainee == null)
            {
                try
                {
                    trainee = RuntimeConfigurator.Configuration.Trainee.GameObject;
                }
                catch (NullReferenceException)
                {
                    return;
                }
            }
            
            currentSpectatorInstance.transform.SetPositionAndRotation(
                Vector3.Lerp(currentSpectatorInstance.transform.position, trainee.transform.position, Time.deltaTime),
                Quaternion.Lerp(currentSpectatorInstance.transform.rotation, trainee.transform.rotation, Time.deltaTime)
            );
        }
    }
}
