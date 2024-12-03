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
        BulletSpawnConfig config = SystemAPI.GetSingleton<BulletSpawnConfig>();

        // TODO : 0.25 저 숫자도 플레이어에게서 얻어와서 하도록 하자.
        if (elapsedTime - lastShootTime < 0.25)
            return;

        lastShootTime = elapsedTime;

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (localTransform, playerRangeAttack )
                 in SystemAPI.Query<RefRO<LocalTransform>, RefRO<PlayerRangeAttack>>().WithAll<Player>())
        {
            if (playerRangeAttack.ValueRO.NumOfGuns <= 0)
            {
                break;
            }
            
            float3 playerPosition = localTransform.ValueRO.Position;
            quaternion playerRotation = localTransform.ValueRO.Rotation;
            float3 playerRight = localTransform.ValueRO.Right();

            // 탄환 생성 위치 (플레이어 양옆)
            float3 leftBulletPosition = playerPosition + playerRight * -1.5f;
            float3 rightBulletPosition = playerPosition + playerRight * 1.5f;

            if (playerRangeAttack.ValueRO.NumOfGuns == 1)
            {
                CreateBullet(ecb, (leftBulletPosition + rightBulletPosition) / 2.0f, playerRotation);
                break;
            }
            // 개수에 따라서 중간에 생성.
            int bulletSections = playerRangeAttack.ValueRO.NumOfGuns - 1;
            float3 deltaPosition = (rightBulletPosition - leftBulletPosition) / bulletSections;
            for (int i = 0; i < playerRangeAttack.ValueRO.NumOfGuns; i++)
            {
                CreateBullet(ecb, leftBulletPosition + deltaPosition * i , playerRotation);
            }
            
            break;
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