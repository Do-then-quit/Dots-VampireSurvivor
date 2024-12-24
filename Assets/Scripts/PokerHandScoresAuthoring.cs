using System;
using Unity.Collections;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

public class PokerHandScoresAuthoring : MonoBehaviour
{
    private class PokerHandScoresBaker : Baker<PokerHandScoresAuthoring>
    {
        public override void Bake(PokerHandScoresAuthoring authoring)
        {
            var entity = GetEntity(authoring, TransformUsageFlags.None);
            
            // 굳이 여기에 해야할까?
            // 그냥 무언가 start할때 createentity하면 땡아닌가?
            AddComponent(entity, new PokerHandScoresSingleton()
            {
                BaseScores = new FixedList128Bytes<int> { 5, 10, 20, 30, 30, 35, 40, 60, 100 },
                Multipliers = new FixedList128Bytes<float> { 1.0f, 2.0f, 2.0f, 3.0f, 4.0f, 4.0f, 4.0f, 7.0f, 8.0f }
            });
        }
    }

    private void OnDestroy()
    {
        // destroy data?
    }
}

public struct PokerHandScoresSingleton : IComponentData
{
    public FixedList128Bytes<int> BaseScores;
    public FixedList128Bytes<float> Multipliers;

}