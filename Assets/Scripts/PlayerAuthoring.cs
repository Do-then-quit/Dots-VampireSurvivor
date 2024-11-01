using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlayerAuthoring : MonoBehaviour
{
    class Baker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            // GetEntity는 게임 오브젝트에서 베이크된 엔티티를 반환합니다.
            var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
            AddComponent(entity, new BasicStatus
            {
                health = 100.0f,
                moveSpeed = 10.0f,
                isActive = true
            });
            AddComponent(entity, new Player());
            AddComponent(entity, new PlayerAttack
            {
                AttackCooldown = 1.0f,
                AttackDamage = 40.0f,
                AttackRange = 5.0f,
                TimeSinceLastAttack = 0.0f,
            });
        }
    }
}

public struct Player : IComponentData
{

}

public struct PlayerAttack : IComponentData
{
    public float AttackCooldown;
    public float AttackRange;
    public float AttackDamage;
    public float TimeSinceLastAttack;
}
