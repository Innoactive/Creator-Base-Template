using System.Runtime.Serialization;
using Innoactive.Creator.Core;
using Innoactive.Creator.Core.Attributes;
using Innoactive.Creator.Core.Conditions;
using Innoactive.Creator.Core.Properties;
using Innoactive.Creator.Core.SceneObjects;
using Innoactive.Creator.Template.Properties;
using Newtonsoft.Json;

namespace Innoactive.Creator.Template.Conditions
{
    [DataContract(IsReference = true)]
    // Condition which is completed when Pointer points at Target.
    public class PointedCondition : Condition<PointedCondition.EntityData>
    {
        [DisplayName("Point at Object")]
        public class EntityData : IConditionData
        {
            [DataMember]
            // Reference to a pointer property.
            public ScenePropertyReference<PointingProperty> Pointer { get; set; }

            [DisplayName("Target with a collider")]
            [DataMember]
            // Reference to a target property.
            public ScenePropertyReference<ColliderWithTriggerProperty> Target { get; set; }

            public bool IsCompleted { get; set; }

            [DataMember]
            [HideInTrainingInspector]
            public string Name { get; set; }

            public Metadata Metadata { get; set; }
        }

        [JsonConstructor]
        // Make sure that references are initialized.
        public PointedCondition() : this(new ScenePropertyReference<PointingProperty>(), new ScenePropertyReference<ColliderWithTriggerProperty>())
        {
        }

        public PointedCondition(ScenePropertyReference<PointingProperty> pointer, ScenePropertyReference<ColliderWithTriggerProperty> target)
        {
            Data.Pointer = pointer;
            Data.Target = target;
        }

        private class EntityAutocompleter : Autocompleter<EntityData>
        {
            public EntityAutocompleter(EntityData data) : base(data)
            {
            }
        
            /// <inheritdoc />
            public override void Complete()
            {
                Data.Pointer.Value.FastForwardPoint(Data.Target);
            }
        }

        private class ActiveProcess : BaseActiveProcessOverCompletable<EntityData>
        {
            private bool wasPointed;
            private ColliderWithTriggerProperty target;
            
            public ActiveProcess(EntityData data) : base(data)
            {
            }

            /// <inheritdoc />
            public override void Start()
            {
                wasPointed = false;
                target = Data.Target.Value;
                Data.IsCompleted = false;

                Data.Pointer.Value.PointerEnter += OnPointerEnter;
            }

            /// <inheritdoc />
            public override void End()
            {
                Data.Pointer.Value.PointerEnter -= OnPointerEnter;
            }

            /// <inheritdoc />
            protected override bool CheckIfCompleted()
            {
                return wasPointed;
            }

            private void OnPointerEnter(ColliderWithTriggerProperty pointed)
            {
                if (target == pointed)
                {
                    wasPointed = true;
                }
            }
        }
        
        /// <inheritdoc />
        public override IProcess GetActiveProcess()
        {
            return new ActiveProcess(Data);
        }

        /// <inheritdoc />
        protected override IAutocompleter GetAutocompleter()
        {
            return new EntityAutocompleter(Data);
        }
    }
}
