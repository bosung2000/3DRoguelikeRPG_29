using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkillState : IEnemyState
{
    Enemy enemy;
    public bool skillEnd;
    public int skillChoice;
    AnimatorStateInfo stateInfo;
    public void EnterState(EnemyController controller)
    {
        enemy = controller.Enemy;

        if(enemy.IsBoss)//보스 스킬
        {
            if(enemy.CurrentPhase == 1)
            {
                //스킬1
            }
            else if(enemy.CurrentPhase == 2)
            {
                skillChoice = Random.Range(0,2);
                if(skillChoice == 0)
                {
                    //스킬1
                }
                else
                {
                    //스킬2
                }
            }
        }
        else//엘리트 스킬
        {
            switch (controller.Enemy.skillB)
            {
                case EnemySkillType.Dash:
                    controller.animator.SetTrigger("Skill_Dash");
                    break;
                case EnemySkillType.SpreadShot:
                    controller.animator.SetTrigger("Skill_SpreadShot");
                    break;
                default:
                    break;
            }
        }

        controller.agent.isStopped = true;
    }

    public void ExitState(EnemyController controller)
    {
        // 엘리트 몬스터일 때 쿨타임 다시 세팅
        if (!controller.Enemy.IsBoss && controller.Enemy.skillB != EnemySkillType.None)
        {
            controller.Enemy.ResetSkillCooldown();  // 이 방식이 더 단순하고 명확함
        }
    }

    public void UpdateState(EnemyController controller)
    {
        stateInfo = controller.animator.GetCurrentAnimatorStateInfo(0);
        skillEnd = false;

        if (controller.Enemy.IsBoss)
        {
            if (enemy.CurrentPhase == 1)
            {
                //스킬1
                //skillEnd = stateInfo.IsName("Skill_Phase1") && stateInfo.normalizedTime >= 1.0f;
            }
            else if (enemy.CurrentPhase == 2)
            {
                //skillEnd = (stateInfo.IsName("Skill_Phase1") || stateInfo.IsName("Skill_Phase2"))
                //    && stateInfo.normalizedTime >= 1.0f;
            }
        }
        else
        {
            switch (controller.Enemy.skillB)
            {
                case EnemySkillType.Dash:
                    skillEnd = stateInfo.IsName("Skill_Dash") && stateInfo.normalizedTime >= 1.0f;
                    break;
                case EnemySkillType.SpreadShot:
                    skillEnd = stateInfo.IsName("Skill_SpreadShot") && stateInfo.normalizedTime >= 1.0f;
                    break;
                    //다른 스킬
            }
        }

        if (skillEnd)
        {
            if (controller.Enemy.IsBoss)
            {
                // 보스 스킬 생략
            }
            else
            {
                // 엘리트 스킬 실행
                switch (controller.Enemy.skillB)
                {
                    case EnemySkillType.Dash:
                        controller.Enemy.SkillDash();
                        break;
                    case EnemySkillType.SpreadShot:
                        break;
                }
            }
            controller.ChageState(EnemyStateType.Chase);
        }
    }
}
