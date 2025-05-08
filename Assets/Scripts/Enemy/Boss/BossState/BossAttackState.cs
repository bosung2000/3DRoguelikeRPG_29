using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackState : IBossState
{
    private Transform _target;
    private int _attackCooldown;
    private int _attackRange;

    private bool _hasAttacked = false;
    public void EnterState(BossController controller)
    {
        _target = GameObject.FindGameObjectWithTag("Player")?.transform;
        _hasAttacked = false;

        _attackCooldown = (int)controller.GetComponent<Boss>().Stat.GetStatValue(EnemyStatType.AttackCooldown);
        _attackRange = (int)controller.GetComponent<Boss>().Stat.GetStatValue(EnemyStatType.AttackRange);
    
        if(_target == null)
        {
            controller.ChageState(BossStateType.KeepDistance);
            return;
        }
    }

    public void ExitState(BossController controller)
    {
        //애니메이션 bool상태 초기화 기능
        controller.animator.ResetTrigger("Attack");
    }

    public void UpdateState(BossController controller)
    {
        if(_target == null)
        {
            controller.ChageState(BossStateType.KeepDistance);
            return;
        }

        float distance = Vector3.Distance(controller.transform.position, _target.position);

        if (_hasAttacked) return;

        //공격 중 플레이어 방향을 바라보게 함
        if(_target != null)
        {
            Vector3 dir = _target.position - controller.transform.position;
            dir.y = 0;
            if(dir != Vector3.zero)
            {
                Quaternion rot = Quaternion.LookRotation(dir);
                controller.transform.rotation = Quaternion.Slerp(controller.transform.rotation, rot, Time.deltaTime * 5f);
            }
        }

    }
}
