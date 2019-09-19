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
    [DisplayName("Point at Object")]
    // Condition which is completed when Pointer points at Target.
    public class PointedCondition : Condition<PointedCondition.EntityData>
    {
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

        private class EntityAutocompleter : IAutocompleter<EntityData>
        {
            public void Complete(EntityData data)
            {
                data.Pointer.Value.FastForwardPoint(data.Target);
            }
        }

        private class ActiveProcess : IStageProcess<EntityData>
        {
            private EntityData data;

            public void Start(EntityData data)
            {
                this.data = data;

                data.Pointer.Value.PointerEnter += OnPointerEnter;
            }

            public IEnumerator Update(EntityData data)
            {
                yield break;
            }

            public void End(EntityData data)
            {
                data.Pointer.Value.PointerEnter -= OnPointerEnter;
                this.data = null;
            }

            public void FastForward(EntityData data)
            {
            }

            private void OnPointerEnter(ColliderWithTriggerProperty pointed)
            {
                if (data.Target.Value == pointed)
                {
                    data.IsCompleted = true;
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
