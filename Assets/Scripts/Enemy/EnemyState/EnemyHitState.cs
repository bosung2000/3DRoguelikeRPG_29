using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitState : IEnemyState
{
    private float _hitDuration = 0.3f; // 경직 시간
    private float _timer = 0f;
    public void EnterState(EnemyController controller)
    {
        _timer = 0f;
        controller.animator.SetTrigger("Hit");
        controller.agent.isStopped = true;
        controller.agent.ResetPath();

        controller.ResetAttackCooldown();
    }

    public void ExitState(EnemyController controller)
    {
    }

    public void UpdateState(EnemyController controller)
    {
        _timer += Time.deltaTime;
        if (_timer > _hitDuration)
        {
            if (controller.GetCurrentHP() <= 0)
            {
                controller.ChageState(EnemyStateType.Dead);
            }
            else
            {
                switch(controller.LastStateType)
                {
                    case EnemyStateType.Chase:
                        controller.ChageState(EnemyStateType.Chase);
                        break;
                    case EnemyStateType.KeepDistance:
                        controller.ChageState(EnemyStateType.KeepDistance);
                        break;
                    default:
                        controller.ChageState(EnemyStateType.Idle);
                        break;
                }
            }
        }
    }
}