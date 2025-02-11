using UnityEngine;
using Unity.Entities;

public class ShopUIController : MonoBehaviour
{
    public GameObject shopUI; // 상점 UI 패널

    void Update()
    {
        var world = World.DefaultGameObjectInjectionWorld;
        var entityManager = world.EntityManager;
        var roundManagerEntityQuery = entityManager.CreateEntityQuery(typeof(RoundManager));
        if (roundManagerEntityQuery.CalculateEntityCount() == 0)
        {
            return;
        }

        if (!roundManagerEntityQuery.HasSingleton<RoundManager>()) return;
        
        var roundManager = world.EntityManager.CreateEntityQuery(typeof(RoundManager))
            .GetSingleton<RoundManager>();
        bool isInShop = roundManager.State == GameState.InShop;
        if (shopUI != null)
            shopUI.SetActive(isInShop);
        // TODO : needs to pause.
    }

    // "다음 레벨" 버튼에서 호출
    public void OnNextLevelButtonClicked()
    {
        var world = World.DefaultGameObjectInjectionWorld;
        var entityManager = world.EntityManager;
        var roundManagerEntityQuery = entityManager.CreateEntityQuery(typeof(RoundManager));
        
        if (roundManagerEntityQuery.CalculateEntityCount() == 0)
            return;
        var roundManager = world.EntityManager.CreateEntityQuery(typeof(RoundManager))
            .GetSingleton<RoundManager>();
        if (roundManagerEntityQuery.HasSingleton<RoundManager>())
        {
            roundManager.RoundNumber += 1;
            roundManager.State = GameState.InBattle;
            roundManager.HasSpawnedEnemies = false;  // 다음 라운드에 적을 다시 스폰할 수 있도록 리셋
            entityManager.SetComponentData(roundManagerEntityQuery.GetSingletonEntity(), roundManager);
            Debug.Log("다음 라운드 시작!");
            shopUI.SetActive(false);
            // pause 해제. 
        }
    }
}