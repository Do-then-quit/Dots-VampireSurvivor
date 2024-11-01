using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlayerAttackEffect : MonoBehaviour
{
    [SerializeField] GameObject attackSprite;

    private void Start()
    {
        //Todo : why this line doesn't execute????
        // 이벤트 구독
        Debug.Log("Subscribe Start");
        PlayerAttackSystem playerAttackSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<PlayerAttackSystem>();
        playerAttackSystem.OnAttack += PlayEffect;
        Debug.Log("Subscribe");
    }

    // Update is called once per frame
    
    private void PlayEffect(object sender, EventArgs e)
    {
        // 이펙트 재생 로직. 여기서 Particle System이나 간단한 애니메이션을 재생할 수 있음.
        Debug.Log("Player Attack Effect Played!");
        // 예시: 이펙트를 활성화하여 잠깐 보여주는 방식
        attackSprite.SetActive(true);
        Invoke(nameof(DeactivateEffect), 0.2f); // 이펙트가 0.5초 동안 보이도록 설정
    }

    private void DeactivateEffect()
    {
        Debug.Log("Deactivate PlayerAttack Effect");
        attackSprite.SetActive(false);
    }
}
