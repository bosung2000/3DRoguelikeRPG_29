using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyIdleState : IEnemyState
{
    private float _scanRadius;//탐지 범위
    private float _attackRange;
    private float _distance;
    private LayerMask _targetLayer;

    // 정찰/순찰 시스템
    private float _wanderRadius = 8f;  // 이동 가능 범위
    private float _waitTime = 2f;       //도착 후 대기 시간
    private float _waitTimer = 0;       // 대기 시간 체크
    private float _moveTimeout = 5f; // 이동 제한 시간
    private float _moveTimer = 0f;
    private bool _isWaiting = false;
    private float _dist;
    public void EnterState(EnemyController controller)
    {
        _scanRadius = controller.GetStat(EnemyStatType.ChaseRange);
        _attackRange = controller.GetStat(EnemyStatType.AttackRange);
        _targetLayer = LayerMask.GetMask("Player");

        if (controller.animator != null)
        {
            controller.animator.ResetTrigger("Hit");
        }

        controller.agent.isStopped = false;
        //애니메이션 파라미터 초기화
        controller.animator.SetBool("isWalk", false);
        controller.animator.SetBool("isRun", false);

        _waitTimer = 0;
        _isWaiting = true;
    }
    public void ExitState(EnemyController controller)
    {
        controller.animator.SetBool("isWalk", false );
    }
    public void UpdateState(EnemyController controller)
    {
        Collider[] hit = Physics.OverlapSphere(controller.transform.position, _scanRadius, _targetLayer);

        _distance = Vector3.Distance(controller.transform.position, controller.GetTarget().position);

        //추격 최대거리에 도달하면 추격상태 전환
        if (hit.Length > 0)
        {
            if (_distance <= _attackRange)
            {
                controller.ChageState(EnemyStateType.Attack);
            }
            else
            {
                controller.ChageState(EnemyStateType.Chase);
            }
        }

        if (controller.Enemy.IsBoss)
            return;

        _dist = controller.agent.remainingDistance;

        if (!controller.agent.hasPath || controller.agent.pathStatus != NavMeshPathStatus.PathComplete)
        {
            _moveTimer = 0f;
            _isWaiting = false;
            RandMovePos(controller);
            return;
        }

        if (!controller.agent.pathPending && controller.agent.remainingDistance <= 0.5f)
        {
            _moveTimer = 0f;

            if (!_isWaiting)
            {
                _isWaiting = true;
                _waitTimer = 0f;
            }
            else
            {
                _waitTimer += Time.deltaTime;
                if(_waitTimer >= _waitTime)
                {
                    _isWaiting = false;
                    RandMovePos(controller);
                }
            }
        }
        else
        {
            _moveTimer += Time.deltaTime;
            if (_moveTimer > _moveTimeout)
            {
                _moveTimer = 0f;
                _isWaiting = false;
                RandMovePos(controller);
            }
        }

        float dist = controller.agent.remainingDistance;
        bool isWalk = !controller.agent.pathPending && dist > 0.2f;
        controller.animator.SetBool("isWalk", isWalk);
        controller.animator.SetBool("isRun", false);
    }

    private void RandMovePos(EnemyController controller)
    {
        Vector3 randDirection = Random.insideUnitSphere * _wanderRadius;
        randDirection += controller.SpawnPosition;
        randDirection.y = controller.transform.position.y;

        NavMeshHit hit;
        if(NavMesh.SamplePosition(randDirection, out hit, _wanderRadius, NavMesh.AllAreas))
        {
            controller.agent.SetDestination(hit.position);
        }
        else
        {
            Debug.Log("목적지 못 찾음");
        }
    }
}