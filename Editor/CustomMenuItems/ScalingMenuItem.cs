using Innoactive.Creator.Core.Behaviors;
using Innoactive.CreatorEditor.UI;
using UnityEngine;

namespace Innoactive.Hub.Training.Template.Editors
{
    public class ScalingMenuItem : StepInspectorMenu.Item<IBehavior>
    {
        public override GUIContent DisplayedName
        {
            get { return new GUIContent("Innoactive/Scale Object"); }
        }

        public override IBehavior GetNewItem()
        {
            return new ScalingBehavior();
        }
    }
}
