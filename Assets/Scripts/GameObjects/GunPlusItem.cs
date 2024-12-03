using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPlusItem : ItemBase
{
    protected override void OnItemCollected()
    {
        int beforeNumGun = WorldEntityManager.GetComponentData<PlayerRangeAttack>(PlayerEntity).NumOfGuns;
        WorldEntityManager.SetComponentData<PlayerRangeAttack>(PlayerEntity, new PlayerRangeAttack
        {
            NumOfGuns = beforeNumGun + 1,
        });
    }
}
