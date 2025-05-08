using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerStatType
{
    MaxHP,//최대HP
    HP,//현재HP
    MaxMP,//최대MP
    MP,//현재MP
    MoveSpeed,//이동속도
    Attack,//공격력
    DMGReduction,//받는데미지감소
    CriticalChance,//크리확률
    CriticalDamage,//크리데미지
    DashDistance,//대시거리
    DashCooldown,//대시쿨타임
    HitCooldown,//피격시무적시간
    absorp, //피해흡혈
    DMGIncrease, //받는데미지증가
    HPRecovery, //HP자동회복
    MPRecovery,//MP자동회복
    GoldAcquisition,//골드획득량
    SkillColltime,//스킬쿨타임
    AttackSpeed//공격속도
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
    ChaseRange,
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
