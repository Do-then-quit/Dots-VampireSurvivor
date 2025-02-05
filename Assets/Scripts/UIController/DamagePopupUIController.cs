using TMPro;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;

public class DamagePopupUIController : MonoBehaviour
{
    public GameObject DamagePopupPrefab; // 데미지 텍스트 프리팹
    void OnEnable()
    {
        if (!DamageEventManager.DamageQueue.IsCreated)
        {
            DamageEventManager.DamageQueue = new NativeQueue<DamageEvent>(Allocator.Persistent);
        }
    }

    void OnDisable()
    {
        if (DamageEventManager.DamageQueue.IsCreated)
            DamageEventManager.DamageQueue.Dispose();
    }
    void Update()
    {
        // DamageEventQueue를 폴링하여 이벤트가 있다면 UI 생성
        if (DamageEventManager.DamageQueue.IsCreated)
        {
            while (DamageEventManager.DamageQueue.Count > 0)
            {
                DamageEvent damageEvent = DamageEventManager.DamageQueue.Dequeue();
                DisplayDamageUI(damageEvent.Damage, damageEvent.Position, damageEvent.BulletColor);
            }
        }
    }
    
    private void DisplayDamageUI(float damageAmount, float3 startPosition, float4 bulletColor)
    {
        var newUI = Instantiate(DamagePopupPrefab, startPosition, Quaternion.identity);
        var newUIText = newUI.GetComponent<TextMeshPro>();
        if (newUIText != null)
        {
            //Debug.Log(newUIText.text);
            newUIText.text = ((int)damageAmount).ToString();
            newUIText.color = new Color(bulletColor.x, bulletColor.y, bulletColor.z, bulletColor.w);
        }
    }
}