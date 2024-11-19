using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHPUIController : MonoBehaviour
{
    [SerializeField] GameObject playerHPSlider;
    // Start is called before the first frame update
    private void OnEnable()
    {
        var playerTakeDamageOnContactSystem = 
            World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<PlayerTakeDamageOnContactSystem>();
        playerTakeDamageOnContactSystem.OnDamageTaken += UpdateHPSlider;

    }

    private void OnDisable()
    {
        if (World.DefaultGameObjectInjectionWorld == null) return;
        var playerTakeDamageOnContactSystem = 
            World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<PlayerTakeDamageOnContactSystem>();
        playerTakeDamageOnContactSystem.OnDamageTaken -= UpdateHPSlider;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // when player take damage, update hpui
    void UpdateHPSlider(float currentHP, float maxHP)
    {
        if (playerHPSlider == null) return;
        playerHPSlider.GetComponent<Slider>().value = currentHP / maxHP;
    }
}
