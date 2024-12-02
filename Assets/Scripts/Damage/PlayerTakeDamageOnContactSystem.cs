using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(EnemyPlayerFollowSystem))]
[BurstCompile]
public partial class PlayerTakeDamageOnContactSystem : SystemBase
{
    private float _lastExecuteTime;
    public Action<float, float> OnDamageTaken;
    public Action OnPlayerDeath;
    
    protected override void OnCreate()
    {
        _lastExecuteTime = 0.0f; // 초기화
    }
    protected override void OnUpdate()
    {
        // TODO : 0.33f 는 중요한 게임 설정 변수인데 하드코딩되어있다... 추후 수정방법 찾자.
        _lastExecuteTime += SystemAPI.Time.DeltaTime;
        if (_lastExecuteTime < 0.33f)    
        {
            return;
        }
        _lastExecuteTime = 0.0f;
        // Player 위치 가져오기
        float3 playerPosition = float3.zero;
        float playerRadius = 0.5f; // 플레이어의 반지름
        float playerHealth = 100.0f;
        bool playerIsActive = true;

        foreach (var (localTransform, playerStatus) 
                 in SystemAPI.Query<RefRO<LocalTransform>, RefRW<BasicStatus>>().WithAll<Player>())
        {
            playerPosition = localTransform.ValueRO.Position;
            playerRadius = playerStatus.ValueRO.radius;
            playerHealth = playerStatus.ValueRO.health;
            playerIsActive = playerStatus.ValueRO.isActive;
            break;
        }

        // if already dead, don't execute below.
        if (!playerIsActive) return;
        
        bool isPlayerHit = false;
        // 몬스터와 충돌 판정
        foreach (var (enemyTransform, enemyStatus, enemyAttack) 
                 in SystemAPI.Query<RefRO<LocalTransform>, RefRO<BasicStatus>, RefRO<EnemyAttack>>().WithAll<Enemy>())
        {
            float3 enemyPosition = enemyTransform.ValueRO.Position;
            float distance = math.distance(playerPosition, enemyPosition);
            
            if (distance < (playerRadius + enemyStatus.ValueRO.radius))
            {
                // 플레이어 체력 감소
                isPlayerHit = true;
                playerHealth -= enemyAttack.ValueRO.AttackDamage; // 예: 데미지 10
                Debug.Log("Player hit! Health: " + playerHealth);
            }
        }
        
        // // 플레이어 체력 업데이트
        foreach (var playerStatus in SystemAPI.Query<RefRW<BasicStatus>>().WithAll<Player>())
        {
            playerStatus.ValueRW.health = playerHealth;
            // shot player hp ui update.
            if (isPlayerHit)
            {
                OnDamageTaken?.Invoke(playerStatus.ValueRO.health, playerStatus.ValueRO.maxHealth);
            }

            // player dead event.
            if (playerHealth <= 0.0f)
            {
                playerStatus.ValueRW.isActive = false;
                OnPlayerDeath?.Invoke();
            }
            break;
        }

        
    }
}