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
        controller.agent.enabled = true;
    }

    public void ExitState(EnemyController controller)
    {
        controller.agent.isStopped = false;

    }

    public void UpdateState(EnemyController controller)
    {
        _timer += Time.deltaTime;
        if (_timer > _hitDuration)
        {
            if (controller.GetHP() <= 0)
            {
                controller.ChageState(EnemyStateType.Dead);
            }
            else
            {
                controller.ChageState(EnemyStateType.Idle);
            }
        }
    }
}