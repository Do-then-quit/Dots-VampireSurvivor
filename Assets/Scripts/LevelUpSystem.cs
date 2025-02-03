using Unity.Entities;
using UnityEngine;

public partial struct LevelUpSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        
        
        foreach (var (level, stats) 
                 in SystemAPI.Query<RefRW<PlayerLevelComponent>, RefRW<PlayerStatsComponent>>().WithAll<Player>())
        {
            if (level.ValueRW.CurrentExp >= level.ValueRW.MaxExp)
            {
                // 레벨업 처리
                level.ValueRW.CurrentExp -= level.ValueRW.MaxExp;
                level.ValueRW.Level += 1;
                level.ValueRW.MaxExp *= 1.5f;  // 다음 레벨에 필요한 경험치 증가 (예시)
                stats.ValueRW.AvailableStatPoints += 3; // 추가 스탯 포인트 지급

                // 게임을 일시정지 상태로 전환
                //pause.IsPaused = true;

                // 디버그 로그
                Debug.Log($"레벨업! 현재 레벨: {level.ValueRO.Level}, 추가 스탯 포인트: {stats.ValueRO.AvailableStatPoints}");
            }
        }
    }
}