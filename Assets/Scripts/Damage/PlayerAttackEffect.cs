using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlayerAttackEffect : MonoBehaviour
{
    [SerializeField] private GameObject attackSprite;

    private void OnEnable()
    {
        EffectEventSystem.OnPlayAttackEffect += SpawnAttackEffect;
    }

    private void OnDisable()
    {
        EffectEventSystem.OnPlayAttackEffect -= SpawnAttackEffect;
    }

    private void SpawnAttackEffect(Vector3 position)
    {
        // 이펙트 프리팹을 인스턴스화하여 지정 위치에 배치
        GameObject attackSpriteInstance = Instantiate(attackSprite, position, Quaternion.identity);
        
        // 이펙트의 지속시간을 제어하거나 ParticleSystem의 Stop 명령 등을 사용해 자동으로 제거
        Destroy(attackSpriteInstance, 0.5f); // 0.5초 후에 자동 제거
    }
}

public static class EffectEventSystem
{
    // 특정 위치에 이펙트를 재생하는 이벤트
    public static event Action<Vector3> OnPlayAttackEffect;

    public static void PlayAttackEffect(Vector3 position)
    {
        OnPlayAttackEffect?.Invoke(position);
    }
}