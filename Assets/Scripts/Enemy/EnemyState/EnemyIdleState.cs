using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyIdleState : IEnemyState
{
    private float _scanRadius;//탐지 범위
    private LayerMask _targetLayer;

    // 정찰/순찰 시스템
    private float _wanderRadius = 10f;// 이동 가능 범위
    private float _waitTime = 2f; //도착 후 대기 시간
    private float _waitTimer = 0;// 대기 시간 체크
    private bool _isWaiting = false;


    public void EnterState(EnemyController controller)
    {
        Debug.Log("Enemy : Idle 상태 진입");

        _scanRadius = controller.GetStat(EnemyStatType.ChaseRange);
        _targetLayer = LayerMask.GetMask("Player");

        if (controller.animator != null)
        {
            controller.animator.SetBool("isMoving", false);
            controller.animator.ResetTrigger("Hit");
        }

        controller.agent.isStopped = false;
        _waitTimer = 0;
        _isWaiting = true;

}
public void ExitState(EnemyController controller)
    {

    }
    public void UpdateState(EnemyController controller)
    {
        /// <summary>
        /// 플레이어 감지
        /// </summary>
        Collider[] hit = Physics.OverlapSphere(controller.transform.position, _scanRadius, _targetLayer);
        foreach (var hits in hit)
        {
            Debug.Log($"[Idle 감지] 감지된 오브젝트: {hits.name}, Layer: {LayerMask.LayerToName(hits.gameObject.layer)}");
        }
        //추격 최대거리에 도달하면 추격상태 전환
        if (hit.Length > 0)
        {
            Debug.Log("플레이어 발견");
            controller.ChageState(EnemyStateType.Chase);
        }

        if(!controller.agent.pathPending && controller.agent.remainingDistance <= 0.5f)
        {
            if(!_isWaiting)
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
    }
}