using Unity.Entities;
using UnityEngine;

public class ConfigAuthoring : MonoBehaviour
{
    public GameObject EnemyPrefab;
    public int EnemyCount;

    class Baker : Baker<ConfigAuthoring>
    {
        public override void Bake(ConfigAuthoring authoring)
        {
            // 설정 엔티티는 Transform 컴포넌트가 필요하지 않으므로
            // TransformUsageFlags.None을 사용합니다.
            var entity = GetEntity(authoring, TransformUsageFlags.None);
            AddComponent(entity, new Config
            {
                // 프리팹을 엔티티로 베이크합니다. GetEntity는 프리팹 계층 구조의 
                // 루트 엔티티를 반환합니다.
                EnemyPrefab = GetEntity(authoring.EnemyPrefab, TransformUsageFlags.Dynamic),
                EnemyCount = authoring.EnemyCount,
            });
        }
    }
}
public struct Config : IComponentData
{
    public Entity EnemyPrefab;
    public int EnemyCount;
}