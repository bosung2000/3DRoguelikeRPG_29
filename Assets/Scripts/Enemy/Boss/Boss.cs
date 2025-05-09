using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public BossStat Stat { get; private set; }
    public BossStatData statData => Stat?.StatData;
    public BossRoleType roleType;
    public int CurrentHP => (int)Stat.GetStatValue(EnemyStatType.HP);
    public int MaxHP => (int)Stat.GetStatValue(EnemyStatType.MaxHP);
    public bool IsPhase2 {  get;  set; }
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
        if(TryGetComponent(out BossController controller))
        {
            controller.ChageState(BossStateType.Dead);
        }
    }

    public bool ShouldEnterPhase2()
    {
        if(IsPhase2) return false;

        float hpRatio = (float)CurrentHP / MaxHP;
        return hpRatio <= statData.Phase2HpThreshold;
    }

    //투사체 발사
    public void FireProjectile()
    {

    }

    public void TriggerSkill2Effect()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 5f, LayerMask.GetMask("Player"));
        foreach(var hit in hits)
        {
            if(hit.TryGetComponent(out PlayerStat player))
            {
                player.TakeDamage(5);
            }
        }
    }

    public void OnAttackAnimationEnd()
    {
        if (TryGetComponent(out BossController controller) && controller.CurrentStateType == BossStateType.Attack)
        {
            controller.ChageState(BossStateType.KeepDistance);
        }
    }
}
