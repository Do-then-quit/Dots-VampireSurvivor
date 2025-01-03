using System;
using System.Drawing;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public partial struct PlayerBulletDamageSystem : ISystem
{
    
    //public Action<int, float3, float4> OnBulletDamage;

    public void OnUpdate(ref SystemState state)
    {
        SystemAPI.GetComponentLookup<HealthComponent>();
    }

    struct BulletEnemyCollideJob : ICollisionEventsJob
    {
        [ReadOnly] public ComponentLookup<PlayerBullet> BulletLookup;
        [ReadOnly] public ComponentLookup<Enemy> EnemiesLookup;
        [ReadOnly] public ComponentLookup<DamageComponent> DamageComponentLookup;
        public ComponentLookup<HealthComponent> HealthComponentLookup;
        public EntityCommandBuffer ECB;
        public void Execute(CollisionEvent collisionEvent)
        {
            var entityA = collisionEvent.EntityA;
            var entityB = collisionEvent.EntityB;
            
            bool isBulletA = BulletLookup.HasComponent(entityA);
            bool isBulletB = BulletLookup.HasComponent(entityB);
            bool isEnemyA = EnemiesLookup.HasComponent(entityA);
            bool isEnemyB = EnemiesLookup.HasComponent(entityB);
            
            if ((isBulletA && isEnemyB) || (isBulletB && isEnemyA))
            {
                var bullet = isBulletA ? entityA : entityB;
                var enemy = isEnemyA ? entityA : entityB;
                
                
                
                // 적에게 데미지 주기
                if (HealthComponentLookup.HasComponent(enemy))
                {
                    var health = HealthComponentLookup[enemy];
                    health.CurrentHealth -= 10; // 탄환 데미지 예시
                    HealthComponentLookup[enemy] = health;
                }

                // 탄환 삭제하기
                //DamageUIController.OnBulletDamage?.Invoke((int)bulletDamage.ValueRO.Damage, enemyPosition, bulletColor.ValueRO.Value);
            }
            // damage event
            //DamageUIController.OnBulletDamage?.Invoke((int)bulletDamage.ValueRO.Damage, enemyPosition, bulletColor.ValueRO.Value);
            
        }
    }
}
