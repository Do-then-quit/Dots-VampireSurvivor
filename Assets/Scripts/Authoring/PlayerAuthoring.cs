using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
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
            AddComponent(entity, new SizeComponent
            {
                Radius = authoring.playerRadius,
            });
            AddComponent(entity, new HealthComponent
            {
                CurrentHealth = authoring.playerHealth,
                MaxHealth = authoring.playerHealth,
            });
            AddComponent(entity, new MovementComponent
            {
                Speed = 10.0f,
            });
            AddComponent(entity, new IsAliveComponent
            {
                IsAlive = true,
            });
            AddComponent(entity, new PlayerLevelComponent
            {
                CurrentExp = 0.0f,
                Level = 1,
                MaxExp = 100.0f,
                LevelUpUIOn = false,
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
            AddComponent(entity, new PlayerHand
            {
                Cards = new FixedList512Bytes<PokerCard>(),
                HandType = PokerHandType.HighCard,
            });
            AddComponent(entity, new PlayerStatsComponent
            {
                HP = 0,
                Damage = 0,
                Speed = 0,
                AvailableStatPoints = 0,
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

public struct PlayerHand : IComponentData
{
    public FixedList512Bytes<PokerCard> Cards;
    public PokerHandType HandType;
}