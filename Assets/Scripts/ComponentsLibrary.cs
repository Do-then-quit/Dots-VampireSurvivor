using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct SizeComponent : IComponentData
{
    public float Radius;
}
public struct HealthComponent : IComponentData
{
    public float CurrentHealth;
    public float MaxHealth;
}
public struct MovementComponent : IComponentData
{
    public float Speed;
}
public struct IsAliveComponent : IComponentData
{
    public bool IsAlive;
}
public struct DamageComponent : IComponentData
{
    public float Damage;
}
public struct ExpValueComponent : IComponentData
{
    public float ExpValue;
}
public struct PlayerLevelComponent : IComponentData
{
    public int Level;
    public float CurrentExp;
    public float MaxExp;
    public bool LevelUpUIOn;
}
// 플레이어의 스탯과 할당 가능한 추가 포인트
public struct PlayerStatsComponent : IComponentData
{
    public int HP;
    public int Damage;
    public int Speed;
    public int AvailableStatPoints;
}
public struct LeftLifeTimeComponent : IComponentData
{
    public float LifeTime;
}

public struct PausedTag : IComponentData {}

public enum GameState { InBattle, InShop }

public struct RoundManager : IComponentData
{
    public int RoundNumber;
    public GameState State;
    public bool HasSpawnedEnemies;
}