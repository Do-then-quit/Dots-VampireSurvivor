using NUnit.Framework.Constraints;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct BulletMoveSystem : ISystem
{
    private bool isPausedLastFrame;

    public void OnCreate(ref SystemState state)
    {
        isPausedLastFrame = false;
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // float deltaTime = SystemAPI.Time.DeltaTime;
        //
        // foreach (var (localTransform, bullet) 
        //          in SystemAPI.Query<RefRW<LocalTransform>, RefRO<MovementComponent>>().WithAll<PlayerBullet>())
        // {
        //     // 탄환 이동 처리
        //     localTransform.ValueRW.Position += localTransform.ValueRW.Up() * bullet.ValueRO.Speed * deltaTime;
        // }
        
        // Pause 상태의 엔티티는 업데이트하지 않음
        if (SystemAPI.HasSingleton<PausedTag>() && !isPausedLastFrame)
        {
            // last frame paused -> bullet stop.
            foreach (var bulletVelocity in 
                     SystemAPI.Query<RefRW<PhysicsVelocity>>().WithAll<PlayerBullet>())
            {
                bulletVelocity.ValueRW.Linear = new float3(0, 0, 0);
                bulletVelocity.ValueRW.Angular = new float3(0, 0, 0);
            }
            isPausedLastFrame = true;
            return;
        }

        if (!SystemAPI.HasSingleton<PausedTag>() && isPausedLastFrame)
        {
            foreach (var (bulletTransform, bulletMovement, bulletVelocity) in 
                     SystemAPI.Query<RefRO<LocalTransform> ,RefRO<MovementComponent>, RefRW<PhysicsVelocity>>().WithAll<PlayerBullet>())
            {
                bulletVelocity.ValueRW.Linear = bulletTransform.ValueRO.Up() * bulletMovement.ValueRO.Speed;
            }
            isPausedLastFrame = false;
        }
        
    }
}