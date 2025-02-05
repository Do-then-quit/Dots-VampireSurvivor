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
        // Pause 상태의 엔티티는 업데이트하지 않음
        // 시간이 잘 작동안하면 그냥 거리기반으로 하는게 나을수도.
        if (SystemAPI.HasSingleton<PausedTag>()) return;
        
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