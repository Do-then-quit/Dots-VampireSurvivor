using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class DamageUIController : MonoBehaviour
{
    [SerializeField] private GameObject damageUIPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        var playerBulletDamageSystem =
            World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<PlayerBulletDamageSystem>();
        playerBulletDamageSystem.OnBulletDamage += DisplayDamageUI;
    }

    private void OnDisable()
    {
        if (World.DefaultGameObjectInjectionWorld == null) return;
        var playerBulletDamageSystem =
            World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<PlayerBulletDamageSystem>();
        playerBulletDamageSystem.OnBulletDamage -= DisplayDamageUI;
    }


    private void DisplayDamageUI(int damageAmount, float3 startPosition, float4 bulletColor)
    {
        var newUI = Instantiate(damageUIPrefab, startPosition, Quaternion.identity);
        var newUIText = newUI.GetComponent<TextMeshPro>();
        if (newUIText != null)
        {
            //Debug.Log(newUIText.text);
            newUIText.text = damageAmount.ToString();
            newUIText.color = new Color(bulletColor.x, bulletColor.y, bulletColor.z, bulletColor.w);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
