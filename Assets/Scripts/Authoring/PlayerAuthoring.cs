using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlayerAuthoring : MonoBehaviour
{
    public float playerHealth;
    public float playerRadius;
    public float playerAttackDamage;

    public int playerRangeAttackGuns;
    class Baker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            // GetEntity는 게임 오브젝트에서 베이크된 엔티티를 반환합니다.
            var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
            AddComponent(entity, new BasicStatus
            {
                health = authoring.playerHealth,
                maxHealth = authoring.playerHealth,
                moveSpeed = 10.0f,
                isActive = true,
                radius = authoring.playerRadius,
            });
            AddComponent(entity, new Player());
            AddComponent(entity, new PlayerMeleeAttack
            {
                AttackCooldown = 1.0f,
                AttackDamage = authoring.playerAttackDamage,
                AttackRange = 5.0f,
                TimeSinceLastAttack = 0.0f,
            });
            AddComponent(entity, new PlayerRangeAttack
            {
                NumOfGuns = authoring.playerRangeAttackGuns,
            });
        }
    }
}

public struct Player : IComponentData
{

}

public struct PlayerMeleeAttack : IComponentData
{
    public float AttackCooldown;
    public float AttackRange;
    public float AttackDamage;
    public float TimeSinceLastAttack;
}

public struct PlayerRangeAttack : IComponentData
{
    public int NumOfGuns;
}
