using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKeepDistanceState : IEnemyState
{
    private Transform _target;          //타겟(플레이어)
    private float _prederredDistance;   //유지거리
    private float _attackRange;         //공격범위
    private float _moveCooldown = 1.5f;
    private float _moveTimer = 0f;
    public void EnterState(EnemyController controller)
    {
        _target = controller.GetTarget();
        _prederredDistance = controller.GetStat(EnemyStatType.AttackRange);
        _attackRange = _prederredDistance;

        controller.agent.isStopped = false;
        controller.animator?.SetBool("isMoving", true);
        _moveTimer = 0f;
    }

    public void ExitState(EnemyController controller)
    {

    }

    public void UpdateState(EnemyController controller)
    {
        if(_target == null)
        {
            controller.ChageState(EnemyStateType.Idle);
            return;
        }

        float distance = Vector3.Distance(controller.transform.position, _target.position);
        float chaseRange = controller.GetStat(EnemyStatType.ChaseRange);
        float attackCooldown = controller.GetStat(EnemyStatType.AttackCooldown);
        bool hasArrived = !controller.agent.pathPending && controller.agent.remainingDistance <= controller.agent.stoppingDistance;

        //사거리 밖으로 나가면 다시 추격
        if (distance > _attackRange * 1.5f)
        {
            controller.ChageState(EnemyStateType.Chase);
            return;
        }

        //사거리 안이면 좌우 이동(쿨타임으로 설정)
        _moveTimer += Time.deltaTime;
        if(_moveTimer >= _moveCooldown)
        {
            _moveTimer = 0f;
            MoveSide(controller);
        }

        //사거리 안에 있고 시간이 쿨타임이 끝났다면 상태 전환
        if(distance <= _attackRange && Time.time >= controller.lastAttackTime + attackCooldown)
        {
            controller.ChageState(EnemyStateType.Attack);
            return;
        }

        if(distance > chaseRange && hasArrived)
        {
            controller.ChageState(EnemyStateType.Idle);
        }

        //애니메이션 제어
        bool isMoving = !controller.agent.pathPending && controller.agent.remainingDistance > controller.agent.stoppingDistance;
        controller.animator.SetBool("isMoving", isMoving);
    }

    private void MoveSide(EnemyController controller)
    {
        Vector3 toPalyer = _target.position - controller.transform.position;
        Vector3 sideDir = Vector3.Cross(Vector3.up, toPalyer).normalized;
        if(Random.value < 0.5f)
        {
            sideDir = -sideDir;
        }

        Vector3 movePos = controller.transform.position + sideDir * 2f;
        controller.agent.SetDestination(movePos);
    }
}
