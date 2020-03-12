using Innoactive.Hub.Training.Behaviors;
using Innoactive.Hub.Training.Editors.Configuration;
using UnityEngine;

namespace Innoactive.Hub.Training.Template.Editors
{
    public class ConfettiMenuItem : Menu.Item<IBehavior>
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
