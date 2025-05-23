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
            float HPmultiplier = StageManager.Instance != null ? StageManager.Instance.HpMultiplier : 1f;
            float currentMaxHp = Mathf.RoundToInt(statData.MaxHP * HPmultiplier);
            float Attackmultiplier = StageManager.Instance != null ? StageManager.Instance.AttackMultiplier : 1f;
            float currenAttack = Mathf.RoundToInt(statData.Attack * Attackmultiplier);
            float Speedmultiplier = StageManager.Instance != null ? StageManager.Instance.SpeedMultiplier : 1f;
            float currenSpeed = Mathf.RoundToInt(statData.Speed + Speedmultiplier);


            SetStatValue(EnemyStatType.MaxHP, currentMaxHp);
            SetStatValue(EnemyStatType.HP, currentMaxHp);
            SetStatValue(EnemyStatType.Speed, currenSpeed);
            SetStatValue(EnemyStatType.Attack, currenAttack);
            SetStatValue(EnemyStatType.Gold, statData.Gold);
            SetStatValue(EnemyStatType.Soul, statData.Soul);
            SetStatValue(EnemyStatType.AttackRange, statData.AttackRange);
            SetStatValue(EnemyStatType.AttackCooldown, statData.AttackCooldown);
            SetStatValue(EnemyStatType.ChaseRange, statData.ChaseRange);
            SetStatValue(EnemyStatType.DashSpeed, statData.DashSpeed);
        }
    }
}