using System.Collections;
using System.Runtime.Serialization;
using Innoactive.Hub.Training.Attributes;
using Innoactive.Hub.Training.Conditions;
using Innoactive.Hub.Training.SceneObjects;
using Innoactive.Hub.Training.SceneObjects.Properties;
using Newtonsoft.Json;

namespace Innoactive.Hub.Training.Template
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
            Data = new EntityData()
            {
                Pointer = pointer,
                Target = target
            };
        }

        private class EntityAutocompleter : BaseAutocompleter<EntityData>
        {
            public void Complete(EntityData data)
            {
                data.Pointer.Value.FastForwardPoint(data.Target);
                base.Complete(data);
            }
        }

        private class ActiveProcess : IStageProcess<EntityData>
        {
            private bool wasPointed;
            private ColliderWithTriggerProperty target;

            public void Start(EntityData data)
            {
                wasPointed = false;
                target = data.Target.Value;
                data.IsCompleted = false;

                data.Pointer.Value.PointerEnter += OnPointerEnter;
            }

            public IEnumerator Update(EntityData data)
            {
                while (wasPointed == false)
                {
                    yield return null;
                }

                data.IsCompleted = true;
            }

            public void End(EntityData data)
            {
                data.Pointer.Value.PointerEnter -= OnPointerEnter;
            }

            public void FastForward(EntityData data)
            {
            }

            private void OnPointerEnter(ColliderWithTriggerProperty pointed)
            {
                if (target == pointed)
                {
                    wasPointed = true;
                }
            }
        }

        private readonly IProcess<EntityData> process = new Process<EntityData>(new EmptyStageProcess<EntityData>(), new ActiveProcess(), new EmptyStageProcess<EntityData>());

        protected override IProcess<EntityData> Process
        {
            get
            {
                return process;
            }
        }

        private readonly IAutocompleter<EntityData> autocompleter = new EntityAutocompleter();

        protected override IAutocompleter<EntityData> Autocompleter
        {
            get
            {
                return autocompleter;
            }
        }
    }
}
