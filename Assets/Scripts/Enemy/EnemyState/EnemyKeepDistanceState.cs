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

        controller.animator?.SetBool("isMoving", true);
        _moveTimer = 0f;
    }

    public void ExitState(EnemyController controller)
    {
        controller.animator?.SetBool("isMoving", false);
        controller.agent.ResetPath();
    }

    public void UpdateState(EnemyController controller)
    {
        if(_target == null)
        {
            controller.ChageState(EnemyStateType.Idle);
            return;
        }

        float distance = Vector3.Distance(controller.transform.position, _target.position);

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
        if(distance <= _attackRange && Time.time >= controller.lastAttackTime + controller.GetStat(EnemyStatType.AttackCooldown))
        {
            controller.ChageState(EnemyStateType.Attack);
        }
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
