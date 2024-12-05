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
                AddComponent(entity, new ExpBubble
                {
                    ExpValue = authoring.expValue,
                });
                AddComponent(entity, new ExpBubbleRadius
                {
                    Radius = authoring.expBubbleRadius,
                });
            }
        }
    }
}

public struct ExpBubble : IComponentData
{
    public float ExpValue;
}

public struct ExpBubbleRadius : IComponentData
{
    public float Radius;
}

