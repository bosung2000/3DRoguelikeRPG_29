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
            controller.ChageState(EnemyStateType.Idle);
            return;
        }

        controller.Enemy.CachedTargetPosition(_target.position);

        controller.agent.isStopped = true; // 근접형은 이동 멈추고 공격
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
        if (stateInfo.IsName("Attack") && stateInfo.normalizedTime < 1.0f)
        {
            return;
        }

        // 애니메이션이 끝났으면 타겟 방향으로 회전
        Vector3 toTarget = _target.position - controller.transform.position;
        toTarget.y = 0f; // 수평만 회전
        if (toTarget != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(toTarget);
            controller.transform.rotation = Quaternion.Slerp(controller.transform.rotation, lookRotation, Time.deltaTime * 10f);
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
        
    }
    private void PerformSupportAttack(EnemyController controller)
    {
    }
}
