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

        controller.agent.isStopped = true;
        controller.animator.SetTrigger("Attack");
    }

    public void ExitState(EnemyController controller)
    {
        controller.animator.SetBool("isMoving", false);
    }

    public void UpdateState(EnemyController controller)
    {
        if (_target == null) return;

        float distance = Vector3.Distance(controller.transform.position, _target.position);
        controller.animator.SetBool("isMoving", false);

        if (distance > attackRange)
        {
            controller.ChageState(EnemyStateType.Chase);
            return;
        }

        if (Time.time >= controller.lastAttackTime + attackCooldown)
        {
            controller.ResetAttackCooldown();

            controller.animator?.SetTrigger("Attack");
            PerformAttack(controller);

            controller.ChageState(EnemyStateType.KeepDistance);
        }
    }

    private void PerformAttack(EnemyController controller)
    {
        switch (controller.Enemy.Role)
        {
            case EnemyRoleType.Melee:
                PerformMeleeAttack(controller);
                break;
            case EnemyRoleType.Ranged:
                PerformRangedAttack(controller);
                break;
            case EnemyRoleType.Support:
                PerformSupportAttack(controller);
                break;
        }

    }

    

    private void PerformMeleeAttack(EnemyController controller)
    {
        if(_target != null)
        {
            float damage = controller.GetAttack();
            _target.GetComponent<PlayerStat>()?.TakeDamage((int)damage);
            Debug.Log($"Melee Attack {damage} 데미지");
        }
    }
    private void PerformRangedAttack(EnemyController controller)
    {
        GameObject prefab = controller.Enemy.ProjectilePrefab;
        Transform firePoint = controller.Enemy.FirePoint;
        if(prefab == null || firePoint == null) return;

        Vector3 targetPos = controller.GetTarget().position;
        Vector3 spawnPos = firePoint.position;//발사 위치

        Vector3 dir = (targetPos - spawnPos).normalized;

        //회전값 계산
        Quaternion rot = Quaternion.LookRotation(dir);

        GameObject projectile = GameObject.Instantiate(prefab, spawnPos, rot);
        Projectile proj = projectile.GetComponent<Projectile>();
        if(proj != null )
        {
            int damage = (int)controller.GetAttack();
            proj.Intialize(dir, damage);
        }
        
        Debug.Log("Ranged Attack 원거리 투사체 발사");
    }
    private void PerformSupportAttack(EnemyController controller)
    {
        Debug.Log("Support Skill 지원 스킬 발동");
    }
}
