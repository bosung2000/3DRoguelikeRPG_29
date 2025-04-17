using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerStatType
{
    MaxHP,
    HP,
    MaxMP,
    MP,
    Speed,
    Attack,
    DMGReduction,
    CriticalChance,
    CriticalDamage,
    DashDistance,
    DashCooltime
}

public enum EnemyStatType
{
    MaxHP,
    HP,
    Speed,
    Attack,
    Gold,
    Soul,//일반 1, 엘리트 5, 보스 10
    AttackRange,
    AttackCooldown,
    ChaseRange
}

public interface IBaseStat<T>
{
    float GetStatValue(T type);
    void SetStatValue(T type, float value);
    void ModifyStat(T type, float amount);
}

public abstract class BaseStat<T> : MonoBehaviour, IBaseStat<T> where T : Enum
{
    protected Dictionary<T, float> stats = new Dictionary<T, float>();

    protected virtual void InitializeStats()
    {
        foreach (T type in Enum.GetValues(typeof(T)))
        {
            stats[type] = 0f;
        }
    }

    public virtual float GetStatValue(T type)
    {
        return stats.TryGetValue(type, out float value) ? value : 0f;
    }

    public virtual void SetStatValue(T type, float value)
    {
        stats[type] = value;
        OnStatChanged();
    }

    public virtual void ModifyStat(T type, float amount)
    {
        if (stats.ContainsKey(type))
        {
            stats[type] += amount;
            OnStatChanged();
        }
    }

    protected virtual void OnStatChanged()
    {

    }
}
