using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKeepDistanceState : IEnemyState
{
    private Transform _target;          //타겟(플레이어)
    private float _attackRange;         //공격범위
    private float _keepDistanceRange;   //대치범위
    private float _stayDuration;        //대치범위 안에 머무는 시간
    private float _stayTimer = 0f;      //머무는 시간 계산
    private float _moveCooldown = 1f;   //대치 이동 쿨타임
    private float _moveTimer = 0f;      //이동쿨타임 계산

    
    public void EnterState(EnemyController controller)
    {
        _target = controller.GetTarget();
        _attackRange = controller.GetStat(EnemyStatType.AttackRange);
        _keepDistanceRange = controller.GetStat(EnemyStatType.KeepDistanceRange);
        _stayDuration = controller.GetStat(EnemyStatType.AttackCooldown);

        _moveTimer = 0f;
        _stayTimer = 0f;

        controller.agent.isStopped = false;
        controller.animator.SetBool("isWalk", true);
        controller.animator.SetBool("isRun", false);

        float baseSpeed = controller.GetStat(EnemyStatType.Speed);
        controller.agent.speed = baseSpeed * 0.5f;
    }

    public void ExitState(EnemyController controller)
    {
        controller.animator.SetBool("isWalk", false);
        controller.agent.speed = controller.GetStat(EnemyStatType.Speed);
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
        if (distance > _keepDistanceRange * 1.5f)
        {
            controller.ChageState(EnemyStateType.Chase);
            return;
        }

        //사거리 안이면 좌우 이동(쿨타임으로 설정)
        if(hasArrived)
        {
            _moveTimer += Time.deltaTime;

            if (_moveTimer >= _moveCooldown)
            {
                _moveTimer = 0f;
                MoveSide(controller);
            }
        }
        

        //사거리 안에 있고 시간이 쿨타임이 끝났다면 상태 전환
        if(distance <= _keepDistanceRange)
        {
            _stayTimer += Time.deltaTime;

            if(_stayTimer >= _stayDuration)
            {
                controller.ChageState(EnemyStateType.Attack);
                return;
            }
        }
        else
        {
            //플레이어가 대치 범위에서 벗어나면 초기화
            _stayTimer = 0f;
        }


        if(distance > _keepDistanceRange && hasArrived)
        {
            controller.ChageState(EnemyStateType.Idle);
        }

        Vector3 toPlayer = _target.position - controller.transform.position;
        toPlayer.y = 0f;

        Quaternion lookRot = Quaternion.LookRotation(toPlayer.normalized);
        Vector3 moveDir = controller.agent.desiredVelocity.normalized;

        if( moveDir.magnitude > 0.1f)
        {
            Vector3 relativeDir = Quaternion.Inverse(lookRot) * moveDir;
            controller.animator.SetFloat("MoveX", relativeDir.x);
            controller.animator.SetFloat("MoveZ", relativeDir.z);
        }
        else
        {
            controller.animator.SetFloat("MoveX", 0);
            controller.animator.SetFloat("MoveZ", 0);
        }

        if(toPlayer != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(toPlayer);
            controller.transform.rotation = Quaternion.Slerp(controller.transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        //애니메이션 제어
        bool isWalk = !controller.agent.pathPending && controller.agent.remainingDistance > controller.agent.stoppingDistance;
        controller.animator.SetBool("isWalk", isWalk);
        controller.animator.SetBool("isRun", false);
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
