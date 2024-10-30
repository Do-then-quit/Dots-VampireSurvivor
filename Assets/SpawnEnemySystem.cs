using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct SpawnEnemySystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SpawnEnemyConfig>();
    }

    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;
        SpawnEnemyConfig spawnEnemyConfig = SystemAPI.GetSingleton<SpawnEnemyConfig>();

        for (int i = 0; i < spawnEnemyConfig.AmountToSpawn; i++)
        {
            Entity spawnedEntity = state.EntityManager.Instantiate(spawnEnemyConfig.EnemyPrefabEntity);
            state.EntityManager.SetComponentData(spawnedEntity, new LocalTransform
            {
                Position = new float3(
                    UnityEngine.Random.Range(-10.0f, 10.0f), 
                    UnityEngine.Random.Range(-10.0f, 10.0f), 
                    0.0f),
                Rotation = quaternion.identity,
                Scale = 1.0f,
            });

        }
    }
}
