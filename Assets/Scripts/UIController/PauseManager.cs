using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;

    private bool isPaused = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        pauseMenuUI.SetActive(true);

        // PausedTag 추가
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        entityManager.CreateEntity(typeof(PausedTag));
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseMenuUI.SetActive(false);

        // PausedTag 제거
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var query = entityManager.CreateEntityQuery(typeof(PausedTag));
        entityManager.DestroyEntity(query);
    }
}
