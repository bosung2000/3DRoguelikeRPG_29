using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossIdleState : IBossState
{
    private float _idleTime = 2f;
    private float _timer = 0f;

    public void EnterState(BossController controller)
    {
        controller.agent.isStopped = true;

        if(controller.animator != null )
        {
            controller.animator.SetBool("isIdle", true);
        }

        _timer = 0f;
    }

    public void ExitState(BossController controller)
    {
        if(controller.animator != null )
        {
            controller.animator.SetBool("isIdle", false);
        }
    }

    public void UpdateState(BossController controller)
    {
        var boss = controller.GetComponent<Boss>();

        if(boss.ShouldEnterPhase2())
        {
            controller.ChageState(BossStateType.Phase2);
            return;
        }

        _timer += Time.deltaTime;

        if(_timer >= _idleTime )
        {
            controller.ChageState(BossStateType.KeepDistance);
        }
    }
}
