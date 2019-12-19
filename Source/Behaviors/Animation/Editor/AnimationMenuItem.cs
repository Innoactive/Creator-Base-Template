using Innoactive.Hub.Training.Behaviors;
using Innoactive.Hub.Training.Editors.Configuration;
using UnityEngine;

namespace Innoactive.Hub.Training.Template.Animation.Editor
{
    public class AnimationMenuItem : Menu.Item<IBehavior>
    {
        public override IBehavior GetNewItem()
        {
            return new AnimationBehavior();
        }

        public override GUIContent DisplayedName
        {
            get { return new GUIContent("Innoactive/Play Animation"); }
        }
    }
}
