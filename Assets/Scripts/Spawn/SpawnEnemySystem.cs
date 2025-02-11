using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct SpawnEnemySystem : ISystem
{
    public Entity EnemyPrefab;
    
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SpawnEnemyConfig>();
        state.RequireForUpdate<RoundManager>();
    }

    public void OnUpdate(ref SystemState state)
    {
        // Pause 상태의 엔티티는 업데이트하지 않음
        if (SystemAPI.HasSingleton<PausedTag>()) return;
        var roundManager = SystemAPI.GetSingleton<RoundManager>();
        EnemyPrefab = SystemAPI.GetSingleton<SpawnEnemyConfig>().EnemyPrefabEntity;

        if (roundManager.State == GameState.InBattle && !roundManager.HasSpawnedEnemies)
        {
            int enemyCount = 5 + roundManager.RoundNumber * 2;
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            for (int i = 0; i < enemyCount; i++)
            {
                Entity enemy = ecb.Instantiate(EnemyPrefab);
                ecb.SetComponent(enemy, new LocalTransform
                {
                    Position = new float3(
                        UnityEngine.Random.Range(-10.0f, 10.0f), 
                        UnityEngine.Random.Range(-10.0f, 10.0f), 
                        0.0f),
                    Rotation = quaternion.identity,
                    Scale = 1.0f,
                });
                
            }
            ecb.Playback(state.EntityManager);
            ecb.Dispose();

            // 이번 라운드에 적을 스폰했음을 표시
            roundManager.HasSpawnedEnemies = true;
            SystemAPI.SetSingleton(roundManager);
        }
    }

    
}
