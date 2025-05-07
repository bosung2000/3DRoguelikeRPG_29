using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class EnemyChaseState : IEnemyState
{
    private Transform _target;
    private float outRangeTime = 0f;
    private float outRangeTimeHold = 2.0f;
    private float _chaseRange;
    private float _attackRange;
    private float _distance;



    public void EnterState(EnemyController controller)
    {
        _target = controller.GetTarget();
        if (_target == null)
        {
            Debug.Log("타겟이 없어 Idle상태로 전환");
            controller.ChageState(EnemyStateType.Idle);
        }

        _chaseRange = controller.GetStat(EnemyStatType.ChaseRange);
        _attackRange = controller.GetStat(EnemyStatType.AttackRange);
        controller.agent.enabled = true;
        controller.agent.isStopped = false;
        controller.agent.angularSpeed = 1000f;
        controller.agent.acceleration = 999f;
        controller.agent.updateRotation = true;

        if (controller.animator != null)
        {
            controller.animator.SetBool("isRun", true);
            controller.animator.SetBool("isWalk", false);
            controller.animator.ResetTrigger("Hit");
        }
    }

    public void ExitState(EnemyController controller)
    {
        controller.animator.SetBool("isRun", false);
    }

    public void UpdateState(EnemyController controller)
    {
        if (_target == null)
        {
            controller.ChageState(EnemyStateType.Idle);
            return;
        }

        //공격 범위 내면 상태 전환용
        _distance = Vector3.Distance(controller.transform.position, _target.position);

        // 범위 밖이면 목적지 계산해서 추적
        if (_distance > _attackRange * 0.9f)
        {
            Vector3 direction = (_target.position - controller.transform.position).normalized;
            Vector3 stopPosition = _target.position - direction * (_attackRange * 0.9f);
            controller.agent.isStopped = false;
            controller.agent.SetDestination(stopPosition);
        }
        else
        {
            // 공격 사거리 안이면 완전히 멈추고 공격 상태로 전환
            controller.agent.isStopped = true;
            controller.ChageState(EnemyStateType.Attack);
            return;
        }

        //범위 밖인 상태 시간 누적
        if (_distance > _chaseRange)
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

        //애니메이션 제어
        bool isRun = !controller.agent.pathPending && controller.agent.remainingDistance > controller.agent.stoppingDistance;
        controller.animator.SetBool("isRun", isRun);
        controller.animator.SetBool("isWalk", false);
    }
}