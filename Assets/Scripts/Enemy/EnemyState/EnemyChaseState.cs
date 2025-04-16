using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseState : IEnemyState
{
    private Transform _target;
    private float stopDistance = 1.5f; //플레리어를 공격하기 전 멈추는 거리

    public void EnterState(EnemyController controller)
    {        
        _target = controller.GetTarget();
        if(_target == null)
        {
            Debug.Log("타겟이 없어 Idle상태로 전환");
            controller.ChageState(EnemyStateType.Idle);
        }
        stopDistance = controller.GetStat(EnemyStatType.AttackRange);

        controller.agent.enabled = true;
        controller.agent.isStopped = false;
        controller.agent.angularSpeed = 1000f;
        controller.agent.acceleration = 999f;
        controller.agent.updateRotation = true;

        controller.animator?.SetBool("isMoving", true);
        Debug.Log("Chase 상태 진입");
    }

    public void ExitState(EnemyController controller)
    {
        controller.animator?.SetBool("isMoving",false);
    }

    public void UpdateState(EnemyController controller)
    {
        if(_target == null)
        {
            return;
        }

        //공격 범위 내면 상태 전환용
        float distance = Vector3.Distance(controller.transform.position, _target.position);

        float chaseRange = controller.GetStat(EnemyStatType.ChaseRange);

        if (distance > chaseRange * 1.5)
        {
            Debug.Log("Chase: 플레이어가 추적 범위를 벗어났습니다.");
            controller.ChageState(EnemyStateType.Idle);
            return;
        }

        float attackRange = controller.GetStat(EnemyStatType.AttackRange);

        //플레이어와 거리가 가까워지면 공격 상태로 전환
        if (distance <= attackRange)
        {
            controller.ChageState(EnemyStateType.Attack);
            return;
        }
        controller.agent.SetDestination(_target.position);


    }
}