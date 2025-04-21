using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStat : BaseStat<PlayerStatType>
{
    public event Action<PlayerStat> OnStatsChanged;

    private Dictionary<PlayerStatType, float> _equipmentBonuses = new Dictionary<PlayerStatType, float>();
    private Dictionary<PlayerStatType, float> _relicBonuses = new Dictionary<PlayerStatType, float>();
    private Dictionary<PlayerStatType, float> _buffBonuses = new Dictionary<PlayerStatType, float>();
    //private Dictionary<PlayerStatType, float> _totalStats = new Dictionary<PlayerStatType, float>();
    private void Awake()
    {
        InitializeStats();
    }
    /// <summary>
    /// 스탯 0으로 초기화
    /// </summary>
    protected override void InitializeStats()
    {
        //stas를 0으로 초기화 
        base.InitializeStats();
        foreach (PlayerStatType type in Enum.GetValues(typeof(PlayerStatType)))
        {
            _equipmentBonuses[type] = 0f;
            _relicBonuses[type] = 0f;
            _buffBonuses[type] = 0f;
        }
    }

    public void InitBaseStat(PlayerStatData playerStatData)
    {
        if (playerStatData != null)
        {
            // ScriptableObject에서 기본값을 가져와 초기화
            foreach (PlayerStatType type in System.Enum.GetValues(typeof(PlayerStatType)))
            {
                float baseValue = playerStatData.GetBaseValue(type);
                SetStatValue(type, baseValue);
            }
            OnStatChanged();
        }
        else
        {
            Debug.LogWarning("PlayerStatData 이 없습니다.");
        }
    }
    public override float GetStatValue(PlayerStatType type)
    {
        float baseValue = base.GetStatValue(type);
        float equipBonus = _equipmentBonuses.TryGetValue(type, out float equip) ? equip : 0f;
        float relicBonus = _relicBonuses.TryGetValue(type, out float relic) ? relic : 0f;
        float buffBonus = _buffBonuses.TryGetValue(type, out float buff) ? buff : 0f;

        return baseValue + equipBonus + relicBonus + buffBonus;
    }

    public void AddEquipmentBonus(Dictionary<PlayerStatType, float> totalconditionTypes)
    {
        foreach (var stat in totalconditionTypes)
        {
            _equipmentBonuses[stat.Key] += stat.Value;
        }
        OnStatChanged();
    }

    /// <summary>
    /// 유물 보너스 추가
    /// </summary>
    /// <param name="bonuses">적용할 유물 보너스</param>
    public void AddRelicBonus(Dictionary<PlayerStatType, float> bonuses)
    {
        foreach (var stat in bonuses)
        {
            _relicBonuses[stat.Key] += stat.Value;
        }
        OnStatChanged();
    }

    /// <summary>
    /// 특정 유물 보너스 확인
    /// </summary>
    /// <param name="type">확인할 스탯 타입</param>
    /// <returns>해당 스탯의 유물 보너스 값</returns>
    public float GetRelicBonus(PlayerStatType type)
    {
        return _relicBonuses.TryGetValue(type, out float value) ? value : 0f;
    }

    public void AddBuff(PlayerStatType type, float bonus)
    {
        if (!_buffBonuses.ContainsKey(type))
        {
            _buffBonuses[type] = 0f;
        }

        _buffBonuses[type] += bonus;
        OnStatChanged();
    }

    protected override void OnStatChanged()
    {
        base.OnStatChanged();
        OnStatsChanged?.Invoke(this);
    }
    /// <summary>
    /// 장비 보너스 스탯 초기화 
    /// </summary>
    internal void ClearEquipmentBonuses()
    {
        foreach (PlayerStatType type in Enum.GetValues(typeof(PlayerStatType)))
        {
            _equipmentBonuses[type] = 0f;
        }
        OnStatsChanged?.Invoke(this);
    }

    /// <summary>
    /// 유물 보너스 스탯 초기화
    /// </summary>
    internal void ClearRelicBonuses()
    {
        foreach (PlayerStatType type in Enum.GetValues(typeof(PlayerStatType)))
        {
            _relicBonuses[type] = 0f;
        }
        OnStatsChanged?.Invoke(this);
    }

    /// <summary>
    /// 모든 보너스 스탯 초기화 (장비, 유물, 버프)
    /// </summary>
    public void ClearAllBonuses()
    {
        foreach (PlayerStatType type in Enum.GetValues(typeof(PlayerStatType)))
        {
            _equipmentBonuses[type] = 0f;
            _relicBonuses[type] = 0f;
            _buffBonuses[type] = 0f;
        }
        OnStatsChanged?.Invoke(this);
    }
}