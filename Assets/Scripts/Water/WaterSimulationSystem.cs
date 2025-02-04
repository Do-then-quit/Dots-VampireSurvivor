using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
// IJobEntity를 사용한 병렬 처리 워터 시뮬레이션 Job
[BurstCompile]
public partial struct WaterSimulationJob : IJobEntity
{
    public float DeltaTime;

    public void Execute(ref WaterNodeComponent node)
    {
        float stiffness = 10f; // 스프링 강도 (조정 가능)
        float springForce = -stiffness * node.Height; // 평형점(0)으로 회귀하는 힘
        node.Velocity += springForce * DeltaTime;
        // 간단한 파동 시뮬레이션: 속도에 감쇠를 곱하고 높이에 반영
        node.Velocity *= node.Damping;
        node.Height += node.Velocity * DeltaTime;
    }
}

[BurstCompile]
public partial struct WaterSimulationSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var job = new WaterSimulationJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime
        };

        // 병렬 처리 실행
        state.Dependency = job.ScheduleParallel(state.Dependency);
    }
}

