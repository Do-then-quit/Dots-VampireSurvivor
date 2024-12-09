using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Mathematics.Geometry;
using Unity.Transforms;
using UnityEngine;

public abstract class ItemBase : MonoBehaviour
{
    protected Entity PlayerEntity;
    protected EntityManager WorldEntityManager;
    // Start is called before the first frame update
    private IEnumerator Start()
    {
        // entity system 이 로드되기까지 0.2초 기다리기... 나중에는 엔티티시스템이 로드되는 이벤트가 있다면 그걸 찾아서 쓰자.
        WorldEntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        yield return new WaitForSeconds(0.2f);
        var playerEntityQuery = WorldEntityManager.CreateEntityQuery(typeof(Player));
        if (playerEntityQuery.HasSingleton<Player>())
        {
            PlayerEntity = playerEntityQuery.GetSingletonEntity();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerEntity == Entity.Null)
        {
            var playerEntityQuery = WorldEntityManager.CreateEntityQuery(typeof(Player));
            if (playerEntityQuery.HasSingleton<Player>())
            {
                PlayerEntity = playerEntityQuery.GetSingletonEntity();
            }
            else
            {
                return;
            }
        }
        // get player location and compare item location
        float3 playerPosition = WorldEntityManager.GetComponentData<LocalTransform>(PlayerEntity).Position;
        // later change this code with item's size.
        float collisionDetectDistance = WorldEntityManager.GetComponentData<SizeComponent>(PlayerEntity).Radius + 1.0f;
        Vector3 playerVector = new Vector3(playerPosition.x, playerPosition.y, playerPosition.z);
        float sqrDistance = (transform.position - playerVector).sqrMagnitude;
        
        // collide -> item function on.
        if (sqrDistance <= collisionDetectDistance)
        {
            OnItemCollected();
            Destroy(gameObject);
        }
    }
    protected abstract void OnItemCollected();
}
