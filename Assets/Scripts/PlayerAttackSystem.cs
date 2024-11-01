using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Events;

[BurstCompile]
public partial class PlayerAttackSystem : SystemBase
{
    public event EventHandler OnAttack;

    protected override void OnUpdate()
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        
        // 플레이어 공격 컴포넌트 쿼리
        foreach (var (attackComponent, playerTransform) 
                 in SystemAPI.Query<RefRW<PlayerAttack>, RefRO<LocalTransform>>())
        {
            var playerPosition = playerTransform.ValueRO.Position;
            attackComponent.ValueRW.TimeSinceLastAttack += deltaTime;

            // 공격 대기 시간이 지났을 때
            if (attackComponent.ValueRW.TimeSinceLastAttack >= attackComponent.ValueRO.AttackCooldown)
            {
                attackComponent.ValueRW.TimeSinceLastAttack = 0f; // 공격 시간 초기화

                int i = 0;
                // 적 쿼리: 범위 내 적들에게 데미지 적용
                foreach (var (enemyHealth, enemyTransform) 
                         in SystemAPI.Query<RefRW<BasicStatus>, RefRO<LocalTransform>>().WithAll<Enemy>())
                {
                    if (math.distance(playerPosition, enemyTransform.ValueRO.Position) <= attackComponent.ValueRO.AttackRange)
                    {
                        enemyHealth.ValueRW.health -= attackComponent.ValueRO.AttackDamage;
                        
                        // 간단한 이펙트를 위해 로그를 출력합니다.
                        i++;
                    }
                }
                
                // shoot effect
                OnAttack?.Invoke(null, EventArgs.Empty);
                Debug.Log("Hit Enemy : " + i);
            }
        }
    }
}