using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct EnemyRotationSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // foreach (var (localTransform, enemy) 
        //          in SystemAPI.Query<RefRW<LocalTransform>, RefRO<Enemy>>())
        // {
        //     localTransform.ValueRW = localTransform.ValueRO
        //         .RotateZ(enemy.ValueRO.moveSpeed * SystemAPI.Time.DeltaTime);
        // }
        float3 playerPosition = new float3(0.0f, 0.0f, 0.0f);
        foreach (var playerLocalTransform in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<Player>())
        {
            playerPosition = playerLocalTransform.ValueRO.Position;
            break;
        }
        RotatingEnemyJob rotatingEnemyJob = new RotatingEnemyJob
        {
            deltaTime = SystemAPI.Time.DeltaTime,
            playerPosition = playerPosition,
        };
        rotatingEnemyJob.ScheduleParallel();
        
    }
    
    [BurstCompile]
    [WithNone(typeof(Player))]
    public partial struct RotatingEnemyJob : IJobEntity
    {
        public float deltaTime;
        public float3 playerPosition;
        private void Execute(ref LocalTransform localTransform, in BasicStatus basicStatus)
        {
            float3 moveDirection = playerPosition - localTransform.Position;

            if (math.lengthsq(moveDirection) > 1.0f)
            {
                // Z축 회전만 허용
                float targetAngle = math.degrees(math.atan2(moveDirection.y, moveDirection.x));
                quaternion targetRotation = quaternion.Euler(0, 0, math.radians(targetAngle));
                localTransform.Rotation = targetRotation;
            }
        }
    }
}
