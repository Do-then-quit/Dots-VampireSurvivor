using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
public partial struct EnemyDamageSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        // 체력이 0 이하인 적을 처리합니다.
        var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
        
        foreach (var (enemyHealth, entity) 
                 in SystemAPI.Query<RefRO<BasicStatus>>().WithAll<Enemy>().WithEntityAccess())
        {
            if (enemyHealth.ValueRO.health <= 0f)
            {
                // 사망 효과 추가 (간단히 로그 출력)
                //Debug.Log("Enemy Destroyed!");

                // 적 엔티티 제거
                commandBuffer.DestroyEntity(entity);
            }
        }
        
        commandBuffer.Playback(state.EntityManager);
        commandBuffer.Dispose();
    }
}