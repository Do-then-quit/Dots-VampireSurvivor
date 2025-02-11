using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct RoundEndDetectionSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<RoundManager>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var roundManager = SystemAPI.GetSingleton<RoundManager>();
        if (roundManager.State == GameState.InShop)
        {
            return;
        }
        // 적 엔티티 수를 체크합니다.
        var enemyQuery = SystemAPI.QueryBuilder().WithAll<Enemy>().Build();
        if (enemyQuery.CalculateEntityCount() == 0 && roundManager.HasSpawnedEnemies == true)
        {
            // 모든 적이 제거되었으면 RoundManager 상태를 InShop으로 전환
            if (SystemAPI.HasSingleton<RoundManager>())
            {
                if (roundManager.State == GameState.InBattle)
                {
                    roundManager.State = GameState.InShop;
                    SystemAPI.SetSingleton(roundManager);
                    Debug.Log("모든 적 처치! 상점 모드로 전환합니다.");
                }
            }
        }
    }
}