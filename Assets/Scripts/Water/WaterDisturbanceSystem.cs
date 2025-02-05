using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

// IJobEntity를 사용한 병렬 처리 Job
[BurstCompile]
public partial struct WaterDisturbanceJob : IJobEntity
{
    public float3 PlayerPosition;
    public float DisturbanceRadiusSq;
    public float DisturbanceStrength;

    public void Execute(ref WaterNodeComponent node, in LocalTransform translation)
    {
        float distSq = math.distancesq(translation.Position, PlayerPosition);
        if (distSq < DisturbanceRadiusSq)
        {
            float factor = 1.0f - math.sqrt(distSq) / math.sqrt(DisturbanceRadiusSq);
            node.Velocity += DisturbanceStrength * factor;
        }
    }
}

// ISystem 내에서 Job을 스케줄하는 코드
[BurstCompile]
public partial struct WaterDisturbanceSystem : ISystem
{
    private float3 playerPosition;
    public float DisturbanceRadius;
    public float DisturbanceStrength ;

    public void OnCreate(ref SystemState state)
    {
        playerPosition = float3.zero;
        DisturbanceRadius = 1.5f;
        DisturbanceStrength = 0.3f;
    }

    public void OnUpdate(ref SystemState state)
    {
        // 플레이어의 위치를 가져오기
        foreach (var translation in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<Player>())
        {
            playerPosition = translation.ValueRO.Position;
            break; // 플레이어는 한 개만 존재한다고 가정
        }

        var job = new WaterDisturbanceJob
        {
            PlayerPosition = playerPosition,
            DisturbanceRadiusSq = DisturbanceRadius * DisturbanceRadius,
            DisturbanceStrength = DisturbanceStrength
        };

        state.Dependency = job.ScheduleParallel(state.Dependency);
    }
}