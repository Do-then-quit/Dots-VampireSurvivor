using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(EnemyPlayerFollowSystem))]
[BurstCompile]
public partial struct PlayerTakeDamageOnContactSystem : ISystem
{
    public float lastExecuteTime;
    public void OnCreate(ref SystemState state)
    {
        lastExecuteTime = 0.0f; // 초기화
    }
    public void OnUpdate(ref SystemState state)
    {
        // TODO : 0.33f 는 중요한 게임 설정 변수인데 하드코딩되어있다... 추후 수정방법 찾자.
        lastExecuteTime += SystemAPI.Time.DeltaTime;
        if (lastExecuteTime < 0.33f)    
        {
            return;
        }
        lastExecuteTime = 0.0f;
        // Player 위치 가져오기
        float3 playerPosition = float3.zero;
        float playerRadius = 0.5f; // 플레이어의 반지름
        float playerHealth = 100.0f;

        foreach (var (localTransform, playerStatus) 
                 in SystemAPI.Query<RefRO<LocalTransform>, RefRW<BasicStatus>>().WithAll<Player>())
        {
            playerPosition = localTransform.ValueRO.Position;
            playerRadius = playerStatus.ValueRO.radius;
            playerHealth = playerStatus.ValueRO.health;
            break;
        }

        // 몬스터와 충돌 판정
        foreach (var (enemyTransform, enemyStatus, enemyAttack) 
                 in SystemAPI.Query<RefRO<LocalTransform>, RefRO<BasicStatus>, RefRO<EnemyAttack>>().WithAll<Enemy>())
        {
            float3 enemyPosition = enemyTransform.ValueRO.Position;
            float distance = math.distance(playerPosition, enemyPosition);
            
            if (distance < (playerRadius + enemyStatus.ValueRO.radius))
            {
                // 플레이어 체력 감소
                playerHealth -= enemyAttack.ValueRO.AttackDamage; // 예: 데미지 10
                Debug.Log("Player hit! Health: " + playerHealth);
            }
        }

        // // 플레이어 체력 업데이트
        foreach (var playerStatus in SystemAPI.Query<RefRW<BasicStatus>>().WithAll<Player>())
        {
            playerStatus.ValueRW.health = playerHealth;
            break;
        }
    }
}