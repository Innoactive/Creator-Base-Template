using Innoactive.Creator.Core.Conditions;
using Innoactive.CreatorEditor.UI;
using UnityEngine;

namespace Innoactive.Hub.Training.Template.Editors
{
    public class PointedMenuItem : StepInspectorMenu.Item<ICondition>
    {
        public override GUIContent DisplayedName
        {
            get { return new GUIContent("Innoactive/Point Object"); }
        }

        public override ICondition GetNewItem()
        {
            return new PointedCondition();
        }
    }
}
