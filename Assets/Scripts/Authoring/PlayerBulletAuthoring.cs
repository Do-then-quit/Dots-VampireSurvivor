using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;


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
            AddComponent(entity, new LeftLifeTimeComponent { LifeTime = authoring.bulletLifetime });
            AddComponent(entity, new DamageComponent { Damage = authoring.bulletDamage });
            AddComponent(entity, new MovementComponent { Speed = authoring.bulletSpeed });
            AddComponent(entity, new SizeComponent { Radius = authoring.bulletSize });
            AddComponent(entity, new PlayerBullet());

        }
    }
}
