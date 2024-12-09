using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
[BurstCompile]
public partial struct PlayerMoveSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        // LoadScene이 되어서 다시 전체 씬이 로드 되어도, 이 OnCreate는 다시 실행되지 않는다!
        Debug.Log("OnCreate MoveSystem");
        state.RequireForUpdate<Player>();
    }

    public void OnUpdate(ref SystemState state)
    {
        foreach (var playerAliveComponent in SystemAPI.Query<RefRO<IsAliveComponent>>().WithAll<Player>())
        {
            if (!playerAliveComponent.ValueRO.IsAlive)
            {
                return;
                //state.Enabled = false;
            }
            break;
        }
        
        float deltaTime = SystemAPI.Time.DeltaTime;
        float3 moveVector = float3.zero;
        moveVector.x = Input.GetAxis("Horizontal");
        moveVector.y = Input.GetAxis("Vertical");
        if (moveVector.x * moveVector.x + moveVector.y * moveVector.y > 1.0f)
        {
            // normalize
            moveVector = math.normalize(moveVector);
        }

        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));
        
        float3 mousePosition = new float3(mouseWorldPosition.x, mouseWorldPosition.y, 0f);

        
        // TODO : 플레이어가 한명 뿐인걸 아는데 매번 이렇게 쿼리를 해서 해야하나? 
        //더 좋은 방법을 나중에 찾으면 고치자
        foreach (var (localTransform, playerMovementComponent) 
                 in SystemAPI.Query<RefRW<LocalTransform>, RefRO<MovementComponent>>().WithAll<Player>())
        {
            localTransform.ValueRW = localTransform.ValueRO.Translate(
                moveVector * playerMovementComponent.ValueRO.Speed * deltaTime);
            float3 rotateDirection = localTransform.ValueRO.Position - mousePosition;
            if (math.lengthsq(rotateDirection) > 0.5f)
            {
                // Z축 회전만 허용
                float targetAngle = math.degrees(math.atan2(rotateDirection.y, rotateDirection.x)) + 90.0f;
                quaternion targetRotation = quaternion.Euler(0, 0, math.radians(targetAngle));
                localTransform.ValueRW.Rotation = targetRotation;
            }
            break;
        }
        
        

    }
}
