using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SpawnEnemyConfigAuthoring : MonoBehaviour
{
    public GameObject enemyPrefab;

    public int amountToSpawn;

    private class SpawnEnemyConfigBaker : Baker<SpawnEnemyConfigAuthoring>
    {
        public override void Bake(SpawnEnemyConfigAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new SpawnEnemyConfig
            {
                EnemyPrefabEntity = GetEntity(authoring.enemyPrefab, TransformUsageFlags.Dynamic),
                AmountToSpawn = authoring.amountToSpawn,
            });
        }
    }
}

public struct SpawnEnemyConfig : IComponentData
{
    public Entity EnemyPrefabEntity;

    public int AmountToSpawn;

}