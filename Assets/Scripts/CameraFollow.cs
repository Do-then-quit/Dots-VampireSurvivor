using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Entity playerEntity;
    
    // World를 통해 EntityManager 접근
    private EntityManager entityManager;
    
    // 카메라가 플레이어를 따라가는 오프셋
    public Vector3 offset = new Vector3(0, 0, -10);
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Camera Start");
        
        // TODO : scene이 리로드 될때, GetSingleton이 entity를 못찾아온다. (초기화 순서문제? lateupdate에서 수행해도 마찬가지)
        
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var playerQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<Player>());
        playerEntity = playerQuery.GetSingletonEntity();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // 플레이어가 존재하는지 확인
        if (playerEntity != Entity.Null)
        {
            // 플레이어의 위치 가져오기
            float3 playerPosition = entityManager.GetComponentData<LocalTransform>(playerEntity).Position;
            
            // 카메라 위치를 플레이어 위치 + 오프셋으로 설정
            transform.position = playerPosition + (float3)offset;
            
            // 필요에 따라 카메라가 항상 플레이어를 바라보게 설정 가능
            //transform.LookAt(playerPosition, offset);
        }
        
    }
}
