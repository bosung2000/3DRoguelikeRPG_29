using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDeadState : IBossState
{
    private bool _animationFinished = false;

    public void EnterState(BossController controller)
    {
        controller.agent.isStopped = true;

        if(controller.animator != null )
        {
            controller.animator.SetTrigger("Die");
        }

        _animationFinished = false;
    }

    public void ExitState(BossController controller)
    {

    }

    public void UpdateState(BossController controller)
    {
        if(_animationFinished) return;

        AnimatorStateInfo stateInfo = controller.animator.GetCurrentAnimatorStateInfo(0);
        
        if(stateInfo.IsName("Die") && stateInfo.normalizedTime >= 1.0f)
        {
            _animationFinished = true;

            Object.Destroy(controller.gameObject);
        }
    }
}
