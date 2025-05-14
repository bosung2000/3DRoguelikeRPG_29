using UnityEngine;

public class EnemyAttackState : IEnemyState
{
    private Transform _target;
    int randomAttackIndex;
    public void EnterState(EnemyController controller)
    {
        _target = controller.GetTarget();

        if (_target == null)
        {
            controller.ChageState(EnemyStateType.Idle);
            return;
        }

        controller.Enemy.CachedTargetPosition(_target.position);
        controller.agent.isStopped = true;
        if(controller.Enemy.IsBoss)
        {
            randomAttackIndex = Random.Range(0, 3);
            controller.animator.SetInteger("attackIndex", randomAttackIndex);
            Debug.Log($"{randomAttackIndex}번 애니메이션 실행");
        }
        else
        {
            controller.animator.SetTrigger("Attack");
        }
    }

    public void ExitState(EnemyController controller)
    {
        controller.animator.SetInteger("attackIndex", -1);
        //controller.animator.SetBool("isRun", false);
    }

    public void UpdateState(EnemyController controller)
    {
        if (_target == null) return;

        //animator에서 레이어(0) -> base Layer에서 진행 중인 애니메이션의 정보를 가져옴
        AnimatorStateInfo stateInfo = controller.animator.GetCurrentAnimatorStateInfo(0);

        //이름이 일치한지 확인하고 애니메이션의 진행파악함 시작(0.0), 끝(1.0)
        if (stateInfo.IsName("Attack") || stateInfo.IsName("Attack_0") || stateInfo.IsName("Attack_1") || stateInfo.IsName("Attack_2"))
        {
            if (stateInfo.normalizedTime >= 1.0f)
            {
                //controller.ChageState(EnemyStateType.Chase);
                return;
            }
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
        }
    }

    private void PerformMeleeAttack(EnemyController controller)
    {
        if(_target == null) return;
        
        controller.agent.isStopped = true;
    }
    private void PerformRangedAttack(EnemyController controller)
    {
        
    }
}
