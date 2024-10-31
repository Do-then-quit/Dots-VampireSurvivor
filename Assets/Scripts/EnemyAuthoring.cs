using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


public class EnemyAuthoring : MonoBehaviour
{
    class Baker : Baker<EnemyAuthoring>
    {
        public override void Bake(EnemyAuthoring authoring)
        {
            // GetEntity는 게임 오브젝트에서 베이크된 엔티티를 반환합니다.
            var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
            AddComponent(entity, new BasicStatus
            {
                health = 100.0f,
                moveSpeed = 4.0f,
                isActive = true,
            });
            AddComponent(entity, new Enemy());
        }
    }
}

// 컴포넌트의 크기는 줄일 수록 좋은 것 같다. 쿼리를 할때 덩어리를 불러와서 순회할텐데,
// 덩어리안에 필요한 데이터만 있어야 캐시미스의 확률도 줄듯.
public struct BasicStatus : IComponentData
{
    // 적의 현재 체력
    public float health;
    // 적의 이동 속도
    public float moveSpeed;
    // 적이 활성화되어 있는지 여부
    public bool isActive;
}

public struct Enemy : IComponentData
{
}