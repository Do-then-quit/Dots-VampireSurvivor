using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

public partial class ConstrainZAxisSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithAll<PhysicsVelocity>()
            .ForEach((ref PhysicsVelocity velocity, ref LocalTransform localTransform) =>
            {
                // Z축 속도 제거
                velocity.Linear.z = 0;

                // Z축 위치 고정
                localTransform.Position.z = 0;
            }).ScheduleParallel();
    }
}