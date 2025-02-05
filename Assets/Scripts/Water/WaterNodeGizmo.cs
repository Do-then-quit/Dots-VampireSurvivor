using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class WaterNodeGizmo : MonoBehaviour
{
    // Scene 뷰에서 디버깅용으로만 사용
    private EntityManager entityManager;

    void OnDrawGizmos()
    {
        if (World.DefaultGameObjectInjectionWorld == null)
            return;
        
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        // WaterNodeComponent와 Translation을 가진 모든 엔티티를 조회합니다.
        var query = entityManager.CreateEntityQuery(typeof(WaterNodeComponent), typeof(LocalTransform));
        using (var waterNodes = query.ToComponentDataArray<WaterNodeComponent>(Unity.Collections.Allocator.TempJob))
        using (var translations = query.ToComponentDataArray<LocalTransform>(Unity.Collections.Allocator.TempJob))
        {
            for (int i = 0; i < waterNodes.Length; i++)
            {
                var node = waterNodes[i];
                var pos = translations[i].Position;
                // 물 높이가 0보다 크면 파도처럼 파란색, 낮으면 어두운 파란색 등으로 표시
                float normalizedHeight = Mathf.InverseLerp(-1f, 1f, node.Height);
                Gizmos.color = Color.Lerp(Color.blue * 0.5f, Color.cyan, normalizedHeight);
                // 구체의 크기를 높이에 따라 조절
                float size = 0.2f + math.abs(node.Height) * 0.2f;
                Gizmos.DrawSphere(new Vector3(pos.x, pos.y + node.Height, pos.z), size);
            }
        }
    }
}