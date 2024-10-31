using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

public partial struct RotationSystem : ISystem
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

        RotatingEnemyJob rotatingEnemyJob = new RotatingEnemyJob
        {
            deltaTime = SystemAPI.Time.DeltaTime,
        };
        rotatingEnemyJob.ScheduleParallel();
        
    }
    
    [BurstCompile]
    [WithNone(typeof(Player))]
    public partial struct RotatingEnemyJob : IJobEntity
    {
        public float deltaTime;
        public void Execute(ref LocalTransform localTransform, in BasicStatus basicStatus)
        {
            localTransform = localTransform
                .RotateZ(basicStatus.moveSpeed * deltaTime);
        }
    }
}
