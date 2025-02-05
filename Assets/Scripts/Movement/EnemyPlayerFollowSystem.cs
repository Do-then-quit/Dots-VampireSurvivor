using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public partial struct EnemyPlayerFollowSystem : ISystem
{
    

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Pause 상태의 엔티티는 업데이트하지 않음
        if (SystemAPI.HasSingleton<PausedTag>())
        {
            // 속도 0으로 초기화 해야함.
            foreach (var enemyVelocity in SystemAPI.Query<RefRW<PhysicsVelocity>>().WithAll<Enemy>())
            {
                enemyVelocity.ValueRW.Linear = new float3(0, 0, 0);
                enemyVelocity.ValueRW.Angular = new float3(0, 0, 0);
            }
        }
        
         float3 playerPosition = float3.zero;
         // player position 얻을 더 좋은 방법 추후 참고. (Entity Access)
         foreach (var localTransform
                  in SystemAPI.Query<RefRO<LocalTransform>>()
                      .WithAll<Player>())
         {
             playerPosition = localTransform.ValueRO.Position;
             break;
         }
         EnemyChasePlayerJob enemyChasePlayerJob = new EnemyChasePlayerJob
         {
             playerPosition = playerPosition,
             deltaTime = SystemAPI.Time.DeltaTime,
         };
         // 2complete version
        state.Dependency = enemyChasePlayerJob.Schedule(state.Dependency);
        state.Dependency.Complete();
        
        //
        // //separation job set.
        // EntityQuery myQuery = SystemAPI.QueryBuilder().WithAll<LocalTransform>().WithAll<Enemy>().Build();
        // NativeArray<LocalTransform> enemyLocalTransforms = myQuery.ToComponentDataArray<LocalTransform>(Allocator.TempJob);
        // var separationJob = new SeparationJob
        // {
        //     EnemyLocalTransforms = enemyLocalTransforms,
        //     SeparationDistance = 1.5f,
        //     DeltaTime = SystemAPI.Time.DeltaTime,
        // };
        // //2complete version
        // state.Dependency = separationJob.Schedule(state.Dependency);
        // state.Dependency.Complete();
        //
        // // 메모리 정리
        // enemyLocalTransforms.Dispose(state.Dependency);

    }
    
    
    // 어떻게 잡이 모든 엔티티들 중에서 올바른 것들만 조회하고 수정되는거지? 좀더 조사해보자.
    // Because Of IJobEntity, IJobChunk의 조금더 간결한 버전이라서 저런 것들이 지원되나봄.
    [BurstCompile]
    [WithAll(typeof(Enemy))]
    public partial struct EnemyChasePlayerJob : IJobEntity
    {
        public float deltaTime;
        public float3 playerPosition;
        public void Execute(
            ref PhysicsVelocity physicsVelocity , 
            in LocalTransform localTransform, 
            in MovementComponent movement,
            in PhysicsMass physicsMass)
        {
            float3 moveDirection = playerPosition - localTransform.Position;
            if (math.lengthsq(moveDirection.xyz) > 1.0f)
            {
                moveDirection = math.normalize(moveDirection);
            }
            // 적을 움직이는 3가지 방법.
            
            // 1. 힘 계산
            float forceMagnitude = 10.0f; // 적에게 가하는 힘의 크기
            float3 force = moveDirection * forceMagnitude;
            physicsVelocity.Linear += force * physicsMass.InverseMass * deltaTime; 
                // 최대 속도 제한
            float currentSpeed = math.length(physicsVelocity.Linear);
            if (currentSpeed > movement.Speed)
            {
                physicsVelocity.Linear = math.normalize(physicsVelocity.Linear) * movement.Speed;
            }
            
            // 2. 속도 강제 세팅.
            //physicsVelocity.Linear = moveDirection * movement.Speed;
            
            // 3. 위치 세팅
            // localTransform.Position = 
            //     localTransform.Position + 
            //     (moveDirection * movement.Speed * deltaTime);
        }
    }
    
    [BurstCompile]
    [WithAll(typeof(Enemy))]
    private partial struct SeparationJob : IJobEntity
    {
        // 아래 attribute deprecate될 수 있다고 함. 직접 dispose해주자.
        // [DeallocateOnJobCompletion]
        [ReadOnly] public NativeArray<LocalTransform> EnemyLocalTransforms;
        public float SeparationDistance;
        public float DeltaTime;
        
        public void Execute(ref LocalTransform localTransform, in MovementComponent movement)
        {
            float3 separationForce = float3.zero;
            
            // n제곱 알고리즘... 마음에 들진 않는데...
            foreach (var otherTranslation in EnemyLocalTransforms)
            {
                float3 offset = localTransform.Position - otherTranslation.Position;
                float distance = math.length(offset);

                if (distance > 0.0f && distance < SeparationDistance)
                {
                    separationForce += math.normalize(offset) * (SeparationDistance - distance);
                }
            }

            localTransform.Position += separationForce * DeltaTime * movement.Speed * 0.5f;
        }
    }
}
