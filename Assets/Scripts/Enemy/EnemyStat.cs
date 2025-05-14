using System.Buffers.Text;
using UnityEngine;

public class EnemyStat : BaseStat<EnemyStatType>
{
    [SerializeField] private EnemyStatData statData;

    public EnemyStatData StatData => statData;

    private void Awake()
    {
        InitializeStats();

        if (statData != null)
        {
            float multiplier = StageManager.Instance != null ? StageManager.Instance.HpMultiplier : 1f;
            float currentMaxHp = Mathf.RoundToInt(statData.MaxHP * multiplier);

            SetStatValue(EnemyStatType.MaxHP, currentMaxHp);
            SetStatValue(EnemyStatType.HP, currentMaxHp);
            SetStatValue(EnemyStatType.Speed, statData.Speed);
            SetStatValue(EnemyStatType.Attack, statData.Attack);
            SetStatValue(EnemyStatType.Gold, statData.Gold);
            SetStatValue(EnemyStatType.Soul, statData.Soul);
            SetStatValue(EnemyStatType.AttackRange, statData.AttackRange);
            SetStatValue(EnemyStatType.AttackCooldown, statData.AttackCooldown);
            SetStatValue(EnemyStatType.ChaseRange, statData.ChaseRange);
        }
    }
}