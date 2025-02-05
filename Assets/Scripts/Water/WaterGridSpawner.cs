using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class WaterGridSpawner : MonoBehaviour
{
    public int GridWidth = 40;      // 가로 셀 개수
    public int GridHeight = 20;     // 세로 셀 개수
    public float CellSize = 1f;     // 셀 당 크기

    void Start()
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        // 물 노드에 필요한 컴포넌트로 이루어진 아키타입 생성
        EntityArchetype waterNodeArchetype = entityManager.CreateArchetype(
            typeof(WaterNodeComponent),
            typeof(LocalTransform)
        );

        // 그리드 중앙을 (0,0)으로 잡기 위해 오프셋 계산
        float offsetX = (GridWidth - 1) * CellSize * 0.5f;
        float offsetY = (GridHeight - 1) * CellSize * 0.5f;

        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y < GridHeight; y++)
            {
                Entity nodeEntity = entityManager.CreateEntity(waterNodeArchetype);
                float3 pos = new float3(x * CellSize - offsetX, y * CellSize - offsetY, 0);
                entityManager.SetComponentData(nodeEntity, new LocalTransform { Position = pos});
                entityManager.SetComponentData(nodeEntity, new WaterNodeComponent
                {
                    Height = 0f,
                    Velocity = 0f,
                    Damping = 0.98f  // 약간의 감쇠 적용
                });
                Debug.Log(x + ", " + y + ", " + pos.x + ", " + pos.y + ", " + pos.z);
            }
        }
    }
}