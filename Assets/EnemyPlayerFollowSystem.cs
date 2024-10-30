using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public partial struct EnemyPlayerFollowSystem : ISystem
{
    

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float3 playerPosition = float3.zero;
        foreach (var localTransform
                 in SystemAPI.Query<RefRO<LocalTransform>>()
                     .WithAll<Player>())
        {
            playerPosition = localTransform.ValueRO.Position;
            break;
        }
        //
        // // Todo: Separation 로직 을 이 뒤에 붙이자.
        EnemyChasePlayerJob enemyChasePlayerJob = new EnemyChasePlayerJob
        {
            playerPosition = playerPosition,
            deltaTime = SystemAPI.Time.DeltaTime,
        };
        
        enemyChasePlayerJob.Schedule();
        // TODO : 여기 아래 코드가 문제 발생. 종속성문제? 아예 문제?
        // TODO : 아니면 아예 시스템으로 따로 빼고 좀 천천히 해보자. native array 를 만들어서 넣는 곳에서도 문제가 있어보이고
        // Todo: 
        //
        //  int size = 0;
        //  foreach (var localTransform in SystemAPI.Query<RefRO<LocalTransform>>().WithNone<Player>())
        //  {
        //      size += 1;
        //  }
        //
        //  NativeArray<LocalTransform> arrayOfLocalTransforms = new NativeArray<LocalTransform>(size, Allocator.None);
        //
        //  int i = 0;
        //  foreach (var localTransform in SystemAPI.Query<RefRO<LocalTransform>>().WithNone<Player>())
        //  {
        //      arrayOfLocalTransforms[i] = localTransform.ValueRO;
        //      i += 1;
        //  }
        //
        //  var separationJob = new SeparationJob
        //  {
        //      EnemyLocalTransforms = arrayOfLocalTransforms,
        //      SeparationDistance = 1.5f,
        //      DeltaTime = SystemAPI.Time.DeltaTime,
        //      playerPosition = playerPosition,
        //  };
        //
        // separationJob.Schedule();
        
    }
    
    
    // 어떻게 잡이 모든 엔티티들 중에서 올바른 것들만 조회하고 수정되는거지? 좀더 조사해보자.
    [BurstCompile]
    [WithNone(typeof(Player))]
    [WithAll(typeof(BasicStatus))]
    public partial struct EnemyChasePlayerJob : IJobEntity
    {
        public float deltaTime;
        public float3 playerPosition;
        public void Execute(ref LocalTransform localTransform, in BasicStatus basicStatus)
        {
            float3 moveDirection = playerPosition - localTransform.Position;
            if (math.lengthsq(moveDirection.xyz) > 1.0f)
            {
                moveDirection = math.normalize(moveDirection);
            }
            localTransform.Position = 
                localTransform.Position + 
                (moveDirection * basicStatus.moveSpeed * deltaTime);
        }
    }
    
    [BurstCompile]
    [WithNone(typeof(Player))]
    [WithAll(typeof(BasicStatus))]
    private partial struct SeparationJob : IJobEntity
    {
        [DeallocateOnJobCompletion]
        [ReadOnly] public NativeArray<LocalTransform> EnemyLocalTransforms;
        public float SeparationDistance;
        public float DeltaTime;
        public float3 playerPosition;
        
        public void Execute(ref LocalTransform localTransform, in BasicStatus basicStatus)
        {
            float3 moveDirection = playerPosition - localTransform.Position;
            if (math.lengthsq(moveDirection.xyz) > 1.0f)
            {
                moveDirection = math.normalize(moveDirection);
            }
            localTransform.Position = 
                localTransform.Position + 
                (moveDirection * basicStatus.moveSpeed * DeltaTime);
            
            float3 separationForce = float3.zero;
            
            // n제곱 알고리즘... 마음에 들진 않는데...
            foreach (var otherTranslation in EnemyLocalTransforms)
            {
                float3 offset = localTransform.Position - otherTranslation.Position;
                float distance = math.length(offset);

                if (distance < SeparationDistance)
                {
                    separationForce += math.normalize(offset) * (SeparationDistance - distance);
                }
            }

            localTransform.Position += separationForce * DeltaTime * basicStatus.moveSpeed * 0.5f;
        }
    }
}
