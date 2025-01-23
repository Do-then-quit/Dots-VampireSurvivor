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
        // Pause 상태의 엔티티는 업데이트하지 않음
        // 시간이 그냥 흘러갈테니까 이거 처리는 할려면 할수는 있겠다. 투두.
        if (SystemAPI.HasSingleton<PausedTag>()) return;
        
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

        // 하나씩 스폰하지말고, Instantiate메소드로 한번에 많이 소환하고 각 엔티티의 위치를 바꿔주는 식으로 바꿔야 함.
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
