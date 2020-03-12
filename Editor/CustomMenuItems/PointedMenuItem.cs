using Innoactive.Hub.Training.Conditions;
using Innoactive.Hub.Training.Editors.Configuration;
using UnityEngine;

namespace Innoactive.Hub.Training.Template.Editors
{
    public class PointedMenuItem : Menu.Item<ICondition>
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
