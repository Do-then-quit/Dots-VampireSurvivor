using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Scenes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUIController : MonoBehaviour
{
    [SerializeField] private GameObject gameOverUI;
    // Start is called before the first frame update
    private void OnEnable()
    {
        var playerTakeDamageOnContactSystem = 
            World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<PlayerTakeDamageOnContactSystem>();
        playerTakeDamageOnContactSystem.OnPlayerDeath += DisplayGameOverUI;

    }
    private void OnDisable()
    {
        if (World.DefaultGameObjectInjectionWorld == null) return;
        var playerTakeDamageOnContactSystem = 
            World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<PlayerTakeDamageOnContactSystem>();
        playerTakeDamageOnContactSystem.OnPlayerDeath -= DisplayGameOverUI;

    }
    
    // when player dead, display gameover ui
    private void DisplayGameOverUI()
    {
        if (gameOverUI == null) return;
        gameOverUI.SetActive(true);
    }

    // button 에 매핑 할 함수.
    // 현재 씬이 제대로 로드되지 않는다. 다시 씬을 로드하고 시작할때, entity를 못찾는 문제가!. 
    public void RestartGame()
    {
        var entityManager = Unity.Entities.World.DefaultGameObjectInjectionWorld.EntityManager;
        entityManager.DestroyEntity(entityManager.UniversalQuery);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
    }
}
