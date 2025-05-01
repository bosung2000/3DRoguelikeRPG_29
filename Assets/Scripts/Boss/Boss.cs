using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public BossStat Stat { get; private set; }
    public BossStatData statData => Stat?.StatData;

    public int CurrentHP => (int)Stat.GetStatValue(EnemyStatType.HP);
    public int MaxHP => (int)Stat.GetStatValue(EnemyStatType.MaxHP);

    private void Awake()
    {
        Stat = GetComponent<BossStat>();
    }

    public void TakeDamage(int damage)
    {
        if(Stat == null) return;

        Stat.ModifyStat(EnemyStatType.HP, -Mathf.Abs(damage));

        if(CurrentHP <= 0)
        {
            Die();
        }
    }

    public void Die()
    {

    }
    
    public bool ShouldEnterPhase2()
    {
        if(statData == null) return false;
        float hpRatio = (float)CurrentHP / MaxHP;
        return hpRatio <= statData.Phase2HpThreshold;
    }
}
