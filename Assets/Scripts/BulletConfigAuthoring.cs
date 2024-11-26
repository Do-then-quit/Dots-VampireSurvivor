using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class BulletConfigAuthoring : MonoBehaviour
{
    public GameObject playerBulletPrefab;

    private class BulletConfigBaker : Baker<BulletConfigAuthoring>
    {
        public override void Bake(BulletConfigAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new BulletSpawnConfig
            {
                PlayerBulletPrefabEntity = GetEntity(authoring.playerBulletPrefab, TransformUsageFlags.Dynamic),
            });
        }
    }
}
public struct BulletSpawnConfig : IComponentData
{
    public Entity PlayerBulletPrefabEntity;
}