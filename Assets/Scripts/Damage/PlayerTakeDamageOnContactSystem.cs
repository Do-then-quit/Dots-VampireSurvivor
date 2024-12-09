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

        foreach (var (localTransform, 
                     playerSizeComponent, 
                     playerHealthComponent, 
                     isPlayerAliveComponent) 
                 in SystemAPI.Query<
                         RefRO<LocalTransform>, 
                         RefRW<SizeComponent>, 
                         RefRO<HealthComponent>, 
                         RefRO<IsAliveComponent>>()
                     .WithAll<Player>())
        {
            playerPosition = localTransform.ValueRO.Position;
            playerRadius = playerSizeComponent.ValueRO.Radius;
            playerHealth = playerHealthComponent.ValueRO.CurrentHealth;
            playerIsActive = isPlayerAliveComponent.ValueRO.IsAlive;
            break;
        }

        // if already dead, don't execute below.
        if (!playerIsActive) return;
        
        bool isPlayerHit = false;
        // 몬스터와 충돌 판정
        foreach (var (enemyTransform, enemySize, enemyAttack) 
                 in SystemAPI.Query<RefRO<LocalTransform>, RefRO<SizeComponent>, RefRO<DamageComponent>>()
                     .WithAll<Enemy>())
        {
            float3 enemyPosition = enemyTransform.ValueRO.Position;
            float distance = math.distance(playerPosition, enemyPosition);
            
            if (distance < (playerRadius + enemySize.ValueRO.Radius))
            {
                // 플레이어 체력 감소
                isPlayerHit = true;
                playerHealth -= enemyAttack.ValueRO.Damage; // 예: 데미지 10
                Debug.Log("Player hit! Health: " + playerHealth);
            }
        }
        
        // // 플레이어 체력 업데이트
        foreach (var (playerHealthComponent, isPlayerAliveComponent )
                 in SystemAPI.Query<RefRW<HealthComponent>, RefRW<IsAliveComponent>>().WithAll<Player>())
        {
            playerHealthComponent.ValueRW.CurrentHealth = playerHealth;
            // shot player hp ui update.
            if (isPlayerHit)
            {
                OnDamageTaken?.Invoke(
                    playerHealthComponent.ValueRO.CurrentHealth,
                    playerHealthComponent.ValueRO.MaxHealth);
            }

            // player dead event.
            if (playerHealth <= 0.0f)
            {
                isPlayerAliveComponent.ValueRW.IsAlive = false;
                OnPlayerDeath?.Invoke();
            }
            break;
        }

        
    }
}