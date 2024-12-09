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

        foreach (var (lifeTimeComponent, entity) 
                 in SystemAPI.Query<RefRW<LeftLifeTimeComponent>>().WithEntityAccess())
        {
            lifeTimeComponent.ValueRW.LifeTime -= deltaTime;
            if (lifeTimeComponent.ValueRW.LifeTime <= 0f)
            {
                ecb.DestroyEntity(entity); // 탄환 삭제
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}