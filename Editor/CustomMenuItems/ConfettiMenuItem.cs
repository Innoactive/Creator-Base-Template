using Innoactive.Creator.Core.Behaviors;
using Innoactive.CreatorEditor.UI;
using UnityEngine;

namespace Innoactive.Hub.Training.Template.Editors
{
    public class ConfettiMenuItem : StepInspectorMenu.Item<IBehavior>
    {
        public override GUIContent DisplayedName
        {
            get { return new GUIContent("Innoactive/Spawn Confetti"); }
        }

        public override IBehavior GetNewItem()
        {
            return new ConfettiBehavior();
        }
    }
}
