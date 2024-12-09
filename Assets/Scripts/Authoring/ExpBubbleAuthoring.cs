using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Authoring
{
    public class ExpBubbleAuthoring : MonoBehaviour
    {
        [FormerlySerializedAs("ExpValue")] public float expValue;
        [FormerlySerializedAs("ExpBubbleRadius")] public float expBubbleRadius;
        private class ExpBubbleBaker : Baker<ExpBubbleAuthoring>
        {
            public override void Bake(ExpBubbleAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
                AddComponent(entity, new ExpValueComponent
                {
                    ExpValue = authoring.expValue,
                });
                AddComponent(entity, new SizeComponent
                {
                    Radius = authoring.expBubbleRadius,
                });
                AddComponent(entity, new ExpBubble());
            }
        }
    }
}

public struct ExpBubble : IComponentData
{
}
