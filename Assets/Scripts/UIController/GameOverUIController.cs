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
    
    //public SubScene subScene;
    
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
        // var entityManager = Unity.Entities.World.DefaultGameObjectInjectionWorld.EntityManager;
        // entityManager.DestroyEntity(entityManager.UniversalQuery);
        // 적들은 없어진다. 이게 무엇을 의미? entity들이 다 없어지긴 한다는 이야기 같고, unload되기는 되는 것 같음. 
        // 시스템들은 계속 update를 한다. 
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Debug.Log("Load Scene Complete?"); // 호출은 된다. 
        
    }
    

}
