using System;
using System.Drawing;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public partial class PlayerBulletDamageSystem : SystemBase
{
    
    public Action<int, float3, float4> OnBulletDamage;
    protected override void OnUpdate()
    {
        // // change this system. do damage when gets collide event.
        // // 엔티티 커맨드 버퍼 생성 (탄환 제거에 사용)
        // var ecb = new EntityCommandBuffer(Allocator.Temp);
        //
        // foreach (var (bulletTransform, 
        //              bulletDamage, 
        //              bulletSize,
        //              bulletColor,
        //              bulletEntity) 
        //          in SystemAPI.Query<
        //                  RefRO<LocalTransform>, 
        //                  RefRO<DamageComponent>, 
        //                  RefRO<SizeComponent>,
        //                  RefRO<URPMaterialPropertyBaseColor>
        //              >()
        //              .WithAll<PlayerBullet>().WithEntityAccess())
        // {
        //     float3 bulletPosition = bulletTransform.ValueRO.Position;
        //
        //     foreach (var (enemyTransform, enemySize, enemyHealth) 
        //              in SystemAPI.Query<RefRO<LocalTransform>, RefRO<SizeComponent>, RefRW<HealthComponent>>()
        //                  .WithAll<Enemy>())
        //     {
        //         float3 enemyPosition = enemyTransform.ValueRO.Position;
        //         float distanceSquared = math.distancesq(bulletPosition, enemyPosition);
        //
        //         float radiusSum = enemySize.ValueRO.Radius + bulletSize.ValueRO.Radius;
        //         // 충돌 범위 확인
        //         if (distanceSquared <= radiusSum * radiusSum)
        //         {
        //             // 적 체력 감소
        //             enemyHealth.ValueRW.CurrentHealth -= bulletDamage.ValueRO.Damage;
        //
        //             // 적 체력이 0 이하면 처리는 따로 시스템이 있다. 일단은 거기서 처리.(enemydamagesystem)
        //             // TODO : 근데 확실히 데미지를 부여하는 부분에서 이것도 처리하는게 괜찮아 보인다.
        //             // if (enemy.ValueRW.Health <= 0)
        //             // {
        //                     // or event
        //             //     ecb.DestroyEntity(enemyTransform.GetEntity(state));
        //             // }
        //             
        //             // damage event
        //             OnBulletDamage?.Invoke((int)bulletDamage.ValueRO.Damage, enemyPosition, bulletColor.ValueRO.Value);
        //             
        //             // 탄환 제거
        //             ecb.DestroyEntity(bulletEntity);
        //
        //             // 한 탄환은 한 적만 처리
        //             break;
        //         }
        //     }
        // }
        // // 커맨드 버퍼 실행
        // ecb.Playback(EntityManager);
        // ecb.Dispose();
    }
}
