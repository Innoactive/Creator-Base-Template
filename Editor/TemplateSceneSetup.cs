using UnityEngine;

namespace Innoactive.CreatorEditor.BasicTemplate
{
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

