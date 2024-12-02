using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
public partial struct BulletLifetimeSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (lifetime, entity) 
                 in SystemAPI.Query<RefRW<BulletLifetime>>().WithEntityAccess())
        {
            lifetime.ValueRW.RemainingTime -= deltaTime;
            if (lifetime.ValueRW.RemainingTime <= 0f)
            {
                ecb.DestroyEntity(entity); // 탄환 삭제
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}