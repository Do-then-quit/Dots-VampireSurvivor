using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct EnemyPlayerFollowSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float3 playerPosition = float3.zero;
        foreach (var localTransform
                 in SystemAPI.Query<RefRO<LocalTransform>>()
                     .WithAll<Player>())
        {
            playerPosition = localTransform.ValueRO.Position;
            break;
        }

        // Todo: 이 부분을 추후 멀티쓰레드 잡으로 옮기자.
        foreach (var (enemyLocalTransform, enemyBasicStatus) 
                 in SystemAPI.Query<RefRW<LocalTransform>, RefRO<BasicStatus>>()
                     .WithNone<Player>())
        {
            float3 moveDirection = playerPosition - enemyLocalTransform.ValueRO.Position;
            if (math.lengthsq(moveDirection.xyz) > 1.0f)
            {
                moveDirection = math.normalize(moveDirection);
            }
            
            // Todo: 현재는 적들끼리 충돌하지 않으니 점점 겹쳐진다. 추후 방법 찾기.
            enemyLocalTransform.ValueRW.Position = 
                enemyLocalTransform.ValueRO.Position + 
                (moveDirection * enemyBasicStatus.ValueRO.moveSpeed * SystemAPI.Time.DeltaTime);
        }

        // RotatingEnemyJob rotatingEnemyJob = new RotatingEnemyJob
        // {
        //     deltaTime = SystemAPI.Time.DeltaTime,
        // };
        // rotatingEnemyJob.ScheduleParallel();
    }
}
