using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct GetExpBubbleSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Player>();
    }

    public void OnUpdate(ref SystemState state)
    {
        // exp bubble과 플레이어사이의 거리를 측정해야겠다. 
        
        // 플레이어 죽었을 시 비활성화.
        foreach (var playerAliveComponent in SystemAPI.Query<RefRO<IsAliveComponent>>().WithAll<Player>())
        {
            if (!playerAliveComponent.ValueRO.IsAlive)
            {
                return;
                //state.Enabled = false;
                // 나중에는 state를 조절하자. 레벨을 다시 시작하면 이 state를 다시 켜주고.
            }
            break;
        }

        var ecb = new EntityCommandBuffer(Allocator.Temp);
        float totalExp = 0.0f;
        foreach (var (playerLocalTransform, playerLevelComponent, playerSizeComponent)
                 in SystemAPI.Query<RefRO<LocalTransform>, RefRW<PlayerLevelComponent>, RefRO<SizeComponent>>()
                     .WithAll<Player>())
        {
            float3 playerPosition = playerLocalTransform.ValueRO.Position;
            foreach (var (bubbleLocalTransform, 
                         bubbleExpValueComponent, 
                         bubbleSizeComponent, 
                         bubbleEntity) 
                     in SystemAPI.Query<RefRO<LocalTransform>, RefRO<ExpValueComponent>, RefRO<SizeComponent>>()
                         .WithAll<ExpBubble>().WithEntityAccess())
            {
                float3 bubblePosition = bubbleLocalTransform.ValueRO.Position;
                float distanceSquared = math.distancesq(playerPosition, bubblePosition);
                float radiusSum = playerSizeComponent.ValueRO.Radius + bubbleSizeComponent.ValueRO.Radius;
                if (distanceSquared <= radiusSum * radiusSum)
                {
                    // get bubble
                    totalExp += bubbleExpValueComponent.ValueRO.ExpValue;
                    ecb.DestroyEntity(bubbleEntity);
                }
            }

            playerLevelComponent.ValueRW.CurrentExp += totalExp;
            break;
        }
        
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
        
    }
}
