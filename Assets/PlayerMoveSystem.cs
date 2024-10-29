using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct PlayerMoveSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Player>();
    }

    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        float3 moveVector = float3.zero;
        moveVector.x = Input.GetAxis("Horizontal");
        moveVector.y = Input.GetAxis("Vertical");
        if (moveVector.x * moveVector.x + moveVector.y * moveVector.y > 1.0f)
        {
            // normalize
            moveVector = math.normalize(moveVector);
        }
        
        // TODO : 플레이어가 한명 뿐인걸 아는데 매번 이렇게 쿼리를 해서 해야하나? 
        //더 좋은 방법을 나중에 찾으면 고치자
        foreach (var (localTransform, player) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<BasicStatus>>().WithAll<Player>())
        {
            localTransform.ValueRW = localTransform.ValueRO.Translate(moveVector * player.ValueRO.moveSpeed * deltaTime);
            break;
        }


    }
}
