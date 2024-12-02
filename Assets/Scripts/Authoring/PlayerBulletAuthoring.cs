using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct BulletLifetime : IComponentData
{
    public float RemainingTime; // 남은 생명 시간
}
public struct BulletDamage : IComponentData
{
    public float DamageAmount; // 데미지 값
}
public struct BulletMovement : IComponentData
{
    public float Speed;
}
public struct BulletCollision : IComponentData
{
    public float Size;
}

public struct PlayerBullet : IComponentData
{
}

public class PlayerBulletAuthoring : MonoBehaviour
{
    [SerializeField] private float bulletLifetime = 5.0f;
    [SerializeField] private float bulletDamage = 5.0f;
    [SerializeField] private float bulletSpeed = 5.0f;
    [SerializeField] private float bulletSize = 1.0f;
    private class PlayerBulletBaker : Baker<PlayerBulletAuthoring>
    {
        public override void Bake(PlayerBulletAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BulletLifetime { RemainingTime = authoring.bulletLifetime });
            AddComponent(entity, new BulletDamage { DamageAmount = authoring.bulletDamage });
            AddComponent(entity, new BulletMovement { Speed = authoring.bulletSpeed });
            AddComponent(entity, new BulletCollision { Size = authoring.bulletSize });
            AddComponent(entity, new PlayerBullet());

        }
    }
}
