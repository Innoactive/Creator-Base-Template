using System;
using Innoactive.Hub.Training.Configuration;
using UnityEngine;

/// <summary>
/// Puts the parent GameObject to the same position and rotation of the trainee camera.
/// </summary>
public class AttachToTraineeView : MonoBehaviour
{
    private GameObject trainee;

    protected void LateUpdate()
    {
        UpdateCameraPositionAndRotation();
    }

    private void UpdateCameraPositionAndRotation()
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

        transform.SetPositionAndRotation(trainee.transform.position, trainee.transform.rotation);
    }
}