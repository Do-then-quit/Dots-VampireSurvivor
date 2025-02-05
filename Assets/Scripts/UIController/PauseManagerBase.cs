using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PauseManagerBase : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject levelUpUI;

    private bool isPaused = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 레벨업 화면에서는 일시정지 못하게 임시로 막음 ㅋㅋ.
        if (!levelUpUI.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame(pauseMenuUI);
            }
            else
            {
                PauseGame(pauseMenuUI);
            }

            return;
        }
        
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        // playerEntity not set.
        if (!entityManager.CreateEntityQuery(typeof(Player)).HasSingleton<Player>())
        {
            return;
        }

        var playerEntity = entityManager.CreateEntityQuery(typeof(Player)).GetSingletonEntity();
        
        bool isLevelUp = entityManager.GetComponentData<PlayerLevelComponent>(playerEntity).LevelUpUIOn;
        if (isLevelUp)
        {
            PauseGame(levelUpUI);
            var before = entityManager.GetComponentData<PlayerLevelComponent>(playerEntity);
            before.LevelUpUIOn = false;
            entityManager.SetComponentData(playerEntity, before);
        }
    }

    public void ResumeGameByLevelUpUI()
    {
        ResumeGame(levelUpUI);
    }

    public void PauseGame(GameObject ui)
    {
        isPaused = true;
        ui.SetActive(true);

        // PausedTag 추가
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        entityManager.CreateEntity(typeof(PausedTag));
    }

    public void ResumeGame(GameObject ui)
    {
        isPaused = false;
        ui.SetActive(false);

        // PausedTag 제거
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var query = entityManager.CreateEntityQuery(typeof(PausedTag));
        entityManager.DestroyEntity(query);
    }
}
