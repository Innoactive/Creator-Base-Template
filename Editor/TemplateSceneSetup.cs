using UnityEngine;

namespace Innoactive.CreatorEditor.BasicTemplate
{
    /// <summary>
    /// Will be called on OnSceneSetup to add the spectator menu.
    /// </summary>
    public class TemplateSceneSetup : OnSceneSetup
    {
        public override void Setup()
        {
            if (GameObject.Find("[CAMERA_CONFIGURATION]") == null)
            {
                GameObject cameraConfig = (GameObject)GameObject.Instantiate(Resources.Load("CustomCamera/Prefabs/[CAMERA_CONFIGURATION]"));
                cameraConfig.name = "[CAMERA_CONFIGURATION]";
            }
        }
    }
}

