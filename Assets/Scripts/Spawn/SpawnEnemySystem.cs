using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct SpawnEnemySystem : ISystem
{
    private float spawnDeltaTime;
    private float spawnInterval;
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SpawnEnemyConfig>();
        spawnDeltaTime = 0.0f;
        spawnInterval = 5.0f;
    }

    public void OnUpdate(ref SystemState state)
    {
        spawnDeltaTime += SystemAPI.Time.DeltaTime;
        if (spawnDeltaTime > spawnInterval)
        {
            spawnDeltaTime = 0.0f;
            SpawnEnemies(ref state);
        }
    }

    public void SpawnEnemies(ref SystemState state)
    {
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
