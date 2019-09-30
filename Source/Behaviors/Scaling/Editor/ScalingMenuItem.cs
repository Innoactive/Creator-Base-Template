using Innoactive.Hub.Training.Behaviors;
using Innoactive.Hub.Training.Editors.Configuration;
using UnityEngine;

namespace Innoactive.Hub.Training.Template.Editors
{
    public class ScalingMenuItem : Menu.Item<IBehavior>
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
