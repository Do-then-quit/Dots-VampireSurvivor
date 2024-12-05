using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ExpBubbleSpawnConfig : MonoBehaviour
{
    public GameObject expBubblePrefab;
    private class ExpBubbleSpawnConfigBaker : Baker<ExpBubbleSpawnConfig>
    {
        public override void Bake(ExpBubbleSpawnConfig authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new ExpBubbleSpawnConfigData
            {
                ExpBubblePrefabEntity = GetEntity(authoring.expBubblePrefab, TransformUsageFlags.Dynamic),
            });
        }
    }
}

public struct ExpBubbleSpawnConfigData : IComponentData
{
    public Entity ExpBubblePrefabEntity;
}