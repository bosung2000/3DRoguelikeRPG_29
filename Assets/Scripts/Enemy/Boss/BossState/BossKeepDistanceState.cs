using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossKeepDistanceState : IBossState
{
    private Transform _target;
    private float _keepDistance;
    private float _moveSpeed;
    private float _timer;
    private float _decisionInterval = 2.0f;

    public void EnterState(BossController controller)
    {
        Debug.Log("Boss 상태: KeepDistance");

        var boss = controller.GetComponent<Boss>();
        _target = GameObject.FindGameObjectWithTag("Player")?.transform;

        _keepDistance = boss.statData.KeepDistance;  // 예: 5~8
        _moveSpeed = boss.Stat.GetStatValue(EnemyStatType.Speed);

        controller.agent.isStopped = false;
        controller.agent.speed = _moveSpeed;

        controller.animator.SetBool("isRun", true);
    }

    public void UpdateState(BossController controller)
    {
        if (_target == null) return;

        _timer += Time.deltaTime;
        var boss = controller.GetComponent<Boss>();
        float distance = Vector3.Distance(controller.transform.position, _target.position);

        if (distance < _keepDistance - 1f)
        {
            // 너무 가까우면 물러남 (플레이어 반대 방향으로 이동)
            Vector3 backDir = (controller.transform.position - _target.position).normalized;
            controller.agent.SetDestination(controller.transform.position + backDir * 2f);
        }
        else if (distance > _keepDistance + 1f)
        {
            // 너무 멀면 가까이 감
            controller.agent.SetDestination(_target.position);
        }
        else
        {
            // 거리 유지 (좌우 이동, 랜덤 움직임)
            Vector3 sideDir = Vector3.Cross(Vector3.up, (_target.position - controller.transform.position).normalized);
            Vector3 randomSide = (Random.value > 0.5f ? sideDir : -sideDir) * 2f;
            controller.agent.SetDestination(controller.transform.position + randomSide);
        }

        // Phase2 진입 체크
        if (boss.ShouldEnterPhase2())
        {
            controller.ChageState(BossStateType.Phase2);
        }

        if (_timer >= _decisionInterval)
        {
            _timer = 0f;

            int nextAction;
            if (boss.IsPhase2)
            {
                nextAction = Random.Range(0, 3);
            }
            else
            {
                nextAction = Random.Range(0, 2);
            }

            switch (nextAction)
            {
                case 0:
                    controller.ChageState(BossStateType.Attack);
                    break;
                case 1:
                    controller.ChageState(BossStateType.Skill1);
                    break;
                case 2:
                    controller.ChageState(BossStateType.Skill2);
                    break;
            }
        }
    }

    public void ExitState(BossController controller)
    {
        controller.animator.SetBool("isRun", false);
    }
}
