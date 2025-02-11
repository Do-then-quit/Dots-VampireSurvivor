using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


public class RoundManagerAuthoring : MonoBehaviour
{
    public int initialRoundNumber;
    public GameState initialState;
    private bool initialHasSpawnedEnemies = false;
    private class RoundManagerBaker : Baker<RoundManagerAuthoring>
    {
        public override void Bake(RoundManagerAuthoring authoring)
        {
            var entity = GetEntity(authoring, TransformUsageFlags.None);
            AddComponent(entity, new RoundManager
            {
                RoundNumber = authoring.initialRoundNumber,
                State = authoring.initialState,
                HasSpawnedEnemies = authoring.initialHasSpawnedEnemies
            });
        }
    }
}
