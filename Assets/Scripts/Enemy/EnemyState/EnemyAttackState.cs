using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using static UnityEngine.GraphicsBuffer;

public class EnemyAttackState : IEnemyState
{
    private Transform _target;

    public void EnterState(EnemyController controller)
    {
        _target = controller.GetTarget();
        if (_target == null)
        {
            Debug.Log("타겟이 없어 Idle상태로 전환");
            controller.ChageState(EnemyStateType.Idle);
        }

        if (controller.Enemy.Role == EnemyRoleType.Melee)
        {
            controller.agent.isStopped = true; // 근접형은 이동 멈추고 공격
        }
        else
        {
            controller.agent.isStopped = false; // 원거리/지원형은 이동 유지하고 공격
        }

        controller.animator.SetTrigger("Attack");
    }

    public void ExitState(EnemyController controller)
    {
        //controller.animator.SetBool("isRun", false);
    }

    public void UpdateState(EnemyController controller)
    {
        if (_target == null) return;

        //animator에서 레이어(0) -> base Layer에서 진행 중인 애니메이션의 정보를 가져옴
        AnimatorStateInfo stateInfo = controller.animator.GetCurrentAnimatorStateInfo(0);
        
        //이름이 일치한지 확인하고 애니메이션의 진행파악함 시작(0.0), 끝(1.0)
        if(stateInfo.IsName("Attack") && stateInfo.normalizedTime < 1.0f)
        {
            return;
        }

        PerformAttack(controller);
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
        if(_target == null) return;
        
        float attackRange = controller.GetStat(EnemyStatType.AttackRange);
        float distance = Vector3.Distance(controller.transform.position, _target.position);

        //공격 범위 밖이라면
        if(distance >  attackRange)
        {
            controller.agent.isStopped = false;
            controller.agent.SetDestination(_target.position);
        }
        //공격 범위 안이라면
        else
        {
            controller.agent.isStopped = true;
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
        
    }
    private void PerformSupportAttack(EnemyController controller)
    {
    }
}
