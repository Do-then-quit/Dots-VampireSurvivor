using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Authoring;
using Unity.Rendering;
using Unity.Transforms;
using Unity.VisualScripting;

[BurstCompile]
public partial struct PlayerBulletSpawnSystem : ISystem
{
    private double lastShootTime;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BulletSpawnConfig>();
        state.RequireForUpdate<PokerHandScoresSingleton>();
        lastShootTime = SystemAPI.Time.ElapsedTime;
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        double elapsedTime = SystemAPI.Time.ElapsedTime;
        //BulletSpawnConfig config = SystemAPI.GetSingleton<BulletSpawnConfig>();

        // TODO : 0.25 저 숫자도 플레이어에게서 얻어와서 하도록 하자.
        if (elapsedTime - lastShootTime < 0.25)
            return;

        lastShootTime = elapsedTime;

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (localTransform, playerRangeAttack, playerHand )
                 in SystemAPI.Query<RefRO<LocalTransform>, RefRO<PlayerRangeAttack>, RefRO<PlayerHand>>().WithAll<Player>())
        {
            if (playerRangeAttack.ValueRO.NumOfGuns <= 0)
            {
                break;
            }
            
            float3 playerPosition = localTransform.ValueRO.Position;
            quaternion playerRotation = localTransform.ValueRO.Rotation;
            float3 playerRight = localTransform.ValueRO.Right();
            float3 playerUp = localTransform.ValueRO.Up();

            // 탄환 생성 위치 (플레이어 양옆)
            float3 leftBulletPosition = playerPosition + playerRight * -1.5f;
            float3 rightBulletPosition = playerPosition + playerRight * 1.5f;
            
            if (playerHand.ValueRO.Cards.Length <= 0)
            {
                // 기본 불렛 쏘기.
                CreateBullet(ecb, (leftBulletPosition + rightBulletPosition) / 2.0f, playerRotation, playerUp);
            }
            else
            {
                // 개수에 따라서 중간에 생성.
                float3 deltaPosition = (rightBulletPosition - leftBulletPosition) / 4;
                for (int i = 0; i < playerHand.ValueRO.Cards.Length; i++)
                {
                    CreateCardBullet(
                        ecb, 
                        leftBulletPosition + deltaPosition * i , 
                        playerRotation, 
                        playerUp,
                        playerHand.ValueRO.Cards[i],
                        playerHand.ValueRO.HandType);
                }
            }
            break;
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    private void CreateBullet(EntityCommandBuffer ecb, float3 position, quaternion rotation, float3 up)
    {
        BulletSpawnConfig config = SystemAPI.GetSingleton<BulletSpawnConfig>();
        Entity bullet = ecb.Instantiate(config.PlayerBulletPrefabEntity);
        
        ecb.SetComponent(bullet, new LocalTransform
        {
            Position = position,
            Rotation = rotation,
            Scale = 1f
        });
        // 생각해보면 탄환마다 속도가 어떻게 정해질지는 모르는 일.
        // 탄환을 소환할때 그때의 플레이어의 정보에 따라 결정될 것.
        // 탄환 안에는 초기속도를 저장해둘 필요가 없지 않을까 (탄환의 movement중에서 속도가 필요 없을지도)
        float3 bulletVelocity = up * 10.0f;
        ecb.SetComponent(bullet, new PhysicsVelocity
        {
            Linear = bulletVelocity,
            Angular = new float3(0, 0, 0)
        });
    }

    private void CreateCardBullet(EntityCommandBuffer ecb, float3 position, quaternion rotation, float3 up, PokerCard card, PokerHandType handType )
    {
        BulletSpawnConfig config = SystemAPI.GetSingleton<BulletSpawnConfig>();
        Entity bullet = ecb.Instantiate(config.PlayerBulletPrefabEntity);
        ecb.SetComponent(bullet, new LocalTransform
        {
            Position = position,
            Rotation = rotation,
            Scale = 1f
        });
        float4 bulletColor = new float4(1, 1, 1, 1);
        switch (card.Suit)
        {
            case Suit.Clubs:
                bulletColor = new float4(0, 1.0f, 0, 1);
                break;
            case Suit.Diamonds:
                bulletColor = new float4(1.0f, 1.0f, 0, 1);
                break;
            case Suit.Hearts:
                bulletColor = new float4(1.0f, 0.0f, 0, 1);
                break;
            case Suit.Spades:
                bulletColor = new float4(0, 0.0f, 0, 1);
                break;
        }
        ecb.SetComponent(bullet, new URPMaterialPropertyBaseColor
        {
            Value = bulletColor,
        });
        var pokerHandScoresData = SystemAPI.GetSingleton<PokerHandScoresSingleton>();
        float tempDamage = (pokerHandScoresData.BaseScores[(int)handType] + card.Number) 
                           * pokerHandScoresData.Multipliers[(int)handType];
        ecb.SetComponent(bullet, new DamageComponent
        {
            Damage = tempDamage,
        });
        float3 bulletVelocity = up * 10.0f;
        ecb.SetComponent(bullet, new PhysicsVelocity
        {
            Linear = bulletVelocity,
            Angular = new float3(0, 0, 0)
        });
    }
}