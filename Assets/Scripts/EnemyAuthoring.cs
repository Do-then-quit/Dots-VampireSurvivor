using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


public class EnemyAuthoring : MonoBehaviour
{
    public float enemyHealth;
    public float enemyRadius;
    public float enemyAttackDamage;
    class Baker : Baker<EnemyAuthoring>
    {
        public override void Bake(EnemyAuthoring authoring)
        {
            // GetEntity는 게임 오브젝트에서 베이크된 엔티티를 반환합니다.
            var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
            AddComponent(entity, new BasicStatus
            {
                health = authoring.enemyHealth,
                maxHealth = authoring.enemyHealth,
                moveSpeed = 4.0f,
                isActive = true,
                radius = authoring.enemyRadius,
            });
            AddComponent(entity, new EnemyAttack
            {
                AttackDamage = authoring.enemyAttackDamage,
            });
            AddComponent(entity, new Enemy());
        }
    }
}

// 컴포넌트의 크기는 줄일 수록 좋은 것 같다. 쿼리를 할때 덩어리를 불러와서 순회할텐데,
// 덩어리안에 필요한 데이터만 있어야 캐시미스의 확률도 줄듯. 근데 귀찮긴 하네 ㅋㅋ 처음부터 설계를 이쁘게 하면 더 좋겠다.
public struct BasicStatus : IComponentData
{
    // 적의 현재 체력
    public float health;
    // 맥스 체력
    public float maxHealth;
    // 적의 이동 속도
    public float moveSpeed;
    // 적이 활성화되어 있는지 여부
    public bool isActive;
    // 적의 반지름 크기
    public float radius;
}

public struct EnemyAttack : IComponentData
{
    public float AttackDamage;
}

public struct Enemy : IComponentData
{
}