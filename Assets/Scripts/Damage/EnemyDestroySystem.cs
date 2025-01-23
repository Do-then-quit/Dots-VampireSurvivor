using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct EnemyDestroySystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ExpBubbleSpawnConfigData>();
    }

    public void OnUpdate(ref SystemState state)
    {
        // Pause 상태의 엔티티는 업데이트하지 않음
        if (SystemAPI.HasSingleton<PausedTag>()) return;
        
        // 체력이 0 이하인 적을 처리합니다.
        var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
        
        foreach (var (enemyHealth, enemyLocalTransform, entity) 
                 in SystemAPI.Query<RefRO<HealthComponent>, RefRO<LocalTransform>>().WithAll<Enemy>().WithEntityAccess())
        {
            if (enemyHealth.ValueRO.CurrentHealth <= 0f)
            {
                // 사망 효과 추가 (간단히 로그 출력)
                //Debug.Log("Enemy Destroyed!");
                
                // 경험치 알 드랍
                CreateExpBubble(commandBuffer, enemyLocalTransform.ValueRO.Position);

                // 적 엔티티 제거
                commandBuffer.DestroyEntity(entity);
            }
        }
        
        commandBuffer.Playback(state.EntityManager);
        commandBuffer.Dispose();
    }
    
    private void CreateExpBubble(EntityCommandBuffer ecb, float3 position)
    {
        ExpBubbleSpawnConfigData config = SystemAPI.GetSingleton<ExpBubbleSpawnConfigData>();
        Entity expBubble = ecb.Instantiate(config.ExpBubblePrefabEntity);
        ecb.SetComponent(expBubble, new LocalTransform
        {
            Position = position,
            Rotation = quaternion.identity,
            Scale = 0.6f,   // 우선은 하드코딩, 추후 동적으로 연결할 방법을 생각해보자. 이미 생성된 것의 전부를 수정할 필요는 없는데...
        });

    }
}