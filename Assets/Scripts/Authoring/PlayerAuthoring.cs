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

public struct PokerCard
{
    public int Number; // 1 ~ 13
    public Suit Suit;  // 문양 (Spades, Hearts, Diamonds, Clubs)

    public PokerCard(int number, string suitString)
    {
        Number = number;
        // "spades", "hearts", "diamonds", "clubs"
        switch (suitString)
        {
            case "spades":
                Suit = Suit.Spades;
                break;
            case "hearts":
                Suit = Suit.Hearts;
                break;
            case "diamonds":
                Suit = Suit.Diamonds;
                break;
            case "clubs":
                Suit = Suit.Clubs;
                break;
            default:
                Suit = Suit.Clubs;
                break;
        }
    }
}

public enum Suit
{
    Spades,
    Hearts,
    Diamonds,
    Clubs
}
public struct PlayerHand : IComponentData
{
    public FixedList512Bytes<PokerCard> Cards;
}