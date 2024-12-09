using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct BulletMoveSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        foreach (var (localTransform, bullet) 
                 in SystemAPI.Query<RefRW<LocalTransform>, RefRO<MovementComponent>>().WithAll<PlayerBullet>())
        {
            // 탄환 이동 처리
            localTransform.ValueRW.Position += localTransform.ValueRW.Up() * bullet.ValueRO.Speed * deltaTime;
        }
    }
}