using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStat : BaseStat<EnemyStatType>
{
    [SerializeField] private BossStatData _statData;

    public BossStatData StatData => _statData;
    private void Awake()
    {
        InitializeStats();

        if(_statData != null)
        {
            SetStatValue(EnemyStatType.MaxHP, _statData.MaxHP);
            SetStatValue(EnemyStatType.HP, _statData.HP);
            SetStatValue(EnemyStatType.Speed, _statData.Speed);
            SetStatValue(EnemyStatType.Attack, _statData.Attack);
            SetStatValue(EnemyStatType.Gold, _statData.Gold);
            SetStatValue(EnemyStatType.Soul, _statData.Soul);
            SetStatValue(EnemyStatType.AttackRange, _statData.AttackRange);
            SetStatValue(EnemyStatType.AttackCooldown, _statData.AttackCooldown);
            SetStatValue(EnemyStatType.KeepDistanceRange, _statData.KeepDistance);

        }
    }
}
