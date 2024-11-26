using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct PlayerBulletSpawnSystem : ISystem
{
    private double lastShootTime;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BulletSpawnConfig>();
        lastShootTime = SystemAPI.Time.ElapsedTime;
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        double elapsedTime = SystemAPI.Time.ElapsedTime;

        if (elapsedTime - lastShootTime < 0.25)
            return;

        lastShootTime = elapsedTime;

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var localTransform in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<Player>())
        {
            float3 playerPosition = localTransform.ValueRO.Position;
            quaternion playerRotation = localTransform.ValueRO.Rotation;
            float3 playerRight = localTransform.ValueRO.Right();

            // 탄환 생성 위치 (플레이어 양옆)
            float3 leftBulletPosition = playerPosition + playerRight * -1.0f;
            float3 rightBulletPosition = playerPosition + playerRight * 1.0f;

            // 탄환 생성
            CreateBullet(ecb, leftBulletPosition, playerRotation);
            CreateBullet(ecb, rightBulletPosition, playerRotation);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    private void CreateBullet(EntityCommandBuffer ecb, float3 position, quaternion rotation)
    {
        BulletSpawnConfig config = SystemAPI.GetSingleton<BulletSpawnConfig>();
        Entity bullet = ecb.Instantiate(config.PlayerBulletPrefabEntity);

        ecb.SetComponent(bullet, new LocalTransform
        {
            Position = position,
            Rotation = rotation,
            Scale = 1f
        });

    }
}