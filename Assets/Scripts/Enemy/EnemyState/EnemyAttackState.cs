using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using static UnityEngine.GraphicsBuffer;

public class EnemyAttackState : IEnemyState
{
    private Transform _target;
    private float attackRange;
    private float attackCooldown;
    private float lastAttackTime;

    public void EnterState(EnemyController controller)
    {
        _target = controller.GetTarget();
        if (_target == null)
        {
            Debug.Log("타겟이 없어 Idle상태로 전환");
            controller.ChageState(EnemyStateType.Idle);
        }

        attackRange = controller.GetStat(EnemyStatType.AttackRange);
        attackCooldown = controller.GetStat(EnemyStatType.AttackCooldown);
        lastAttackTime = Time.time;

        controller.animator?.SetBool("isMoving", false);
    }

    public void ExitState(EnemyController controller)
    {

    }

    public void UpdateState(EnemyController controller)
    {
        if (_target == null) return;

        float distance = Vector3.Distance(controller.transform.position, _target.position);

        if (distance > attackRange)
        {
            controller.ChageState(EnemyStateType.Chase);
            return;
        }

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;

            controller.animator?.SetTrigger("Attack");
            PerformAttack(controller);
        }
    }

    private void PerformAttack(EnemyController controller)
    {
        float damage = controller.GetAttack();

        switch (controller.Enemy.Role)
        {
            case EnemyRoleType.Melee:

                break;
            case EnemyRoleType.Ranged:
                break;
            case EnemyRoleType.Support:
                break;
        }

        _target.GetComponent<PlayerStat>()?.TakeDamage((int)damage);
        //Debug.Log($"Attack {damage}");
    }

    public void ResetAttackCooldown()
    {
        lastAttackTime = Time.time;
    }
}
