using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseState : IEnemyState
{
    private Transform _target;
    private float outRangeTime = 0f;
    private float outRangeTimeHold = 2.0f;


    public void EnterState(EnemyController controller)
    {
        _target = controller.GetTarget();
        if (_target == null)
        {
            Debug.Log("타겟이 없어 Idle상태로 전환");
            controller.ChageState(EnemyStateType.Idle);
        }

        controller.agent.enabled = true;
        controller.agent.isStopped = false;
        controller.agent.angularSpeed = 1000f;
        controller.agent.acceleration = 999f;
        controller.agent.updateRotation = true;

        if (controller.animator != null)
        {
            controller.animator.SetBool("isMoving", true);
            controller.animator.ResetTrigger("Hit");
        }
    }

    public void ExitState(EnemyController controller)
    {

    }

    public void UpdateState(EnemyController controller)
    {
        if (_target == null)
        {
            controller.ChageState(EnemyStateType.Idle);
            return;
        }

        //공격 범위 내면 상태 전환용
        float distance = Vector3.Distance(controller.transform.position, _target.position);
        float chaseRange = controller.GetStat(EnemyStatType.ChaseRange);
        float attackRange = controller.GetStat(EnemyStatType.AttackRange);
        
        controller.agent.SetDestination(_target.position);

        //범위 밖인 상태 시간 누적
        if (distance > chaseRange )
        {
            outRangeTime += Time.deltaTime;

            if(outRangeTime > outRangeTimeHold)
            {
                controller.ChageState(EnemyStateType.Idle);
                return;
            }
        }
        else
        {
            outRangeTime = 0f; //범위 안이면 초기화
        }

        //플레이어와 거리가 가까워지면 공격 상태로 전환
        if (distance <= attackRange)
        {
            controller.ChageState(EnemyStateType.KeepDistance);
            return;
        }

        //애니메이션 제어
        bool isMoving = !controller.agent.pathPending && controller.agent.remainingDistance > controller.agent.stoppingDistance;
        controller.animator.SetBool("isMoving", isMoving);
    }
}