using Innoactive.Creator.Core.Conditions;
using Innoactive.Creator.Template.Conditions;
using Innoactive.CreatorEditor.UI.StepInspector.Menu;

namespace Innoactive.CreatorEditor.Template.UI.Behaviors
{
    public class PointedMenuItem : MenuItem<ICondition>
    {
        /// <inheritdoc />
        public override string DisplayedName { get; } = "Innoactive/Point Object";

        /// <inheritdoc />
        public override ICondition GetNewItem()
        {
            return new PointedCondition();
        }
    }
}
