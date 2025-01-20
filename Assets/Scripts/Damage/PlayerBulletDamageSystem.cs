using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireMatchingQueriesForUpdate]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
public partial struct BulletCollisionSystem : ISystem
{
    internal ComponentDataHandles m_ComponentDataHandles;
    
    internal struct ComponentDataHandles
    {
        public ComponentLookup<DamageComponent> BulletDataLookup;
        public ComponentLookup<HealthComponent> EnemyHealthLookup;
        
        public ComponentLookup<PlayerBullet> PlayerBulletLookup;
        public ComponentLookup<Enemy> EnemyTagLookup;

        public ComponentDataHandles(ref SystemState systemState)
        {
            BulletDataLookup = systemState.GetComponentLookup<DamageComponent>(true);
            EnemyHealthLookup = systemState.GetComponentLookup<HealthComponent>(false);
            
            PlayerBulletLookup = systemState.GetComponentLookup<PlayerBullet>(true);
            EnemyTagLookup = systemState.GetComponentLookup<Enemy>(true);
        }

        public void Update(ref SystemState systemState)
        {
            BulletDataLookup.Update(ref systemState);
            EnemyHealthLookup.Update(ref systemState);
            
            PlayerBulletLookup.Update(ref systemState);
            EnemyTagLookup.Update(ref systemState);
        }
    }

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // state.RequireForUpdate(state.GetEntityQuery(
        //     ComponentType.ReadWrite<DamageComponent>(),
        //     ComponentType.ReadOnly<HealthComponent>()
        //     ));
        state.RequireForUpdate(state.GetEntityQuery(ComponentType.ReadOnly<PlayerBullet>()));
        state.RequireForUpdate(state.GetEntityQuery(ComponentType.ReadOnly<Enemy>()));

        state.RequireForUpdate<SimulationSingleton>();
        m_ComponentDataHandles = new ComponentDataHandles(ref state);

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        m_ComponentDataHandles.Update(ref state);
        
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

        var simulation = SystemAPI.GetSingleton<SimulationSingleton>();
        state.Dependency = new BulletCollisionEventJob
        {
            BulletDataLookup = m_ComponentDataHandles.BulletDataLookup,
            EnemyHealthLookup = m_ComponentDataHandles.EnemyHealthLookup,
            PlayerBulletLookup = m_ComponentDataHandles.PlayerBulletLookup,
            EnemyTagLookup = m_ComponentDataHandles.EnemyTagLookup,
            entityCommandBuffer = ecb
        }.Schedule(simulation, state.Dependency);
        state.Dependency.Complete();
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    [BurstCompile]
    struct BulletCollisionEventJob : ICollisionEventsJob
    {
        [ReadOnly] public ComponentLookup<DamageComponent> BulletDataLookup;
        public ComponentLookup<HealthComponent> EnemyHealthLookup;
        
        [ReadOnly] public ComponentLookup<PlayerBullet> PlayerBulletLookup;
        [ReadOnly] public ComponentLookup<Enemy> EnemyTagLookup;
        
        public EntityCommandBuffer entityCommandBuffer;

        public void Execute(CollisionEvent collisionEvent)
        {
            Entity entityA = collisionEvent.EntityA;
            Entity entityB = collisionEvent.EntityB;

            bool isEntityABullet = BulletDataLookup.HasComponent(entityA) && PlayerBulletLookup.HasComponent(entityA);
            bool isEntityBBullet = BulletDataLookup.HasComponent(entityB) && PlayerBulletLookup.HasComponent(entityB);

            bool isEntityAEnemy = EnemyHealthLookup.HasComponent(entityA) && EnemyTagLookup.HasComponent(entityA);
            bool isEntityBEnemy = EnemyHealthLookup.HasComponent(entityB) && EnemyTagLookup.HasComponent(entityB);

            if (isEntityABullet && isEntityBEnemy)
            {
                ApplyDamage(entityA, entityB);
            }
            else if (isEntityBBullet && isEntityAEnemy)
            {
                ApplyDamage(entityB, entityA);
            }
            
            
        }

        private void ApplyDamage(Entity bullet, Entity enemy)
        {
            var bulletDamage = BulletDataLookup[bullet];
            var enemyHealth = EnemyHealthLookup[enemy];

            // Apply damage to the enemy
            enemyHealth.CurrentHealth -= bulletDamage.Damage;
            EnemyHealthLookup[enemy] = enemyHealth;


            // Destroy the bullet
            entityCommandBuffer.DestroyEntity(bullet);
        }
    }
}

