using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class EnemySkillState : IEnemyState
{
    public bool skillEnd;
    AnimatorStateInfo stateInfo;
    public void EnterState(EnemyController controller)
    {
        switch (controller.Enemy.skillType)
        {
            case EnemySkillType.Skill1:
                //controller.animator.SetTrigger();
                break;
            default:
                break;
        }
        controller.agent.isStopped = true;
    }

    public void ExitState(EnemyController controller)
    {

    }

    public void UpdateState(EnemyController controller)
    {
        stateInfo = controller.animator.GetCurrentAnimatorStateInfo(0);
        skillEnd = false;
        switch (controller.Enemy.skillType)
        {
            case EnemySkillType.Skill1:
                //스킬 애니메이션 끝나면  skillEnd = true;
                break;
        }

        if(skillEnd)
        {
            controller.ChageState(EnemyStateType.Chase);
        }
    }
}
