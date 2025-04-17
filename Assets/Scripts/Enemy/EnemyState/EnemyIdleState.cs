using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : IEnemyState
{
    private float _scanRadius;//탐지 범위
    private LayerMask _targetLayer;

    public void EnterState(EnemyController controller)
    {
        Debug.Log("Enemy : Idle 상태 진입");

        _scanRadius = controller.GetStat(EnemyStatType.ChaseRange);
        _targetLayer = LayerMask.GetMask("Player");
    
        if(controller.animator != null)
        {
            controller.animator.SetBool("isMoving", false);
            controller.animator.ResetTrigger("Hit");
        }
    }
    public void ExitState(EnemyController controller)
    {
        Debug.Log("Enemy : Idle 상태 종료");
    }
    public void UpdateState(EnemyController controller)
    {
        Collider[] hit = Physics.OverlapSphere(controller.transform.position, _scanRadius, _targetLayer);
        foreach (var hits in hit)
        {
            Debug.Log($"[Idle 감지] 감지된 오브젝트: {hits.name}, Layer: {LayerMask.LayerToName(hits.gameObject.layer)}");
        }
        //추격하기 최소거리에 도달하면 추격상태 전환
        if (hit.Length > 0)
        {
            Debug.Log("플레이어 발견");
            controller.ChageState(EnemyStateType.Chase);
        }
    }
}