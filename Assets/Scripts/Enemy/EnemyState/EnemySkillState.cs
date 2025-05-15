using UnityEngine;

public class EnemySkillState : IEnemyState
{
    Enemy enemy;
    public bool skillEnd;
    AnimatorStateInfo stateInfo;
    public void EnterState(EnemyController controller)
    {
        enemy = controller.Enemy;

        if (controller.animator == null)
        {
            Debug.LogError("Animator가 null임!");
            return;
        }

        if (enemy.IsBoss)//보스 스킬
        {
            if(enemy.CurrentPhase == 1)
            {
                //스킬1
                enemy.CurrentSkillChoice = 0;
            }
            else if(enemy.CurrentPhase == 2)
            {
                enemy.CurrentSkillChoice = Random.Range(0, 2);
            }

            EnemySkillType skill = enemy.GetCurrentSkillType();
            string trigger = enemy.GetSkillTriggerName(skill);
            if(!string.IsNullOrEmpty(trigger))
            {
                controller.animator.SetTrigger(trigger);
            }
        }
        else//엘리트 스킬
        {
            switch (controller.Enemy.skillA)
            {
                case EnemySkillType.Dash:
                    controller.animator.SetTrigger("Skill_Dash");
                    Debug.Log("asdf");
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
        if (!controller.Enemy.IsBoss || controller.Enemy.skillA != EnemySkillType.None)
        {
            controller.Enemy.ResetSkillCooldown();  // 이 방식이 더 단순하고 명확함
            Debug.Log("asdfRESET");
        }

        controller.agent.isStopped = false;
    }

    public void UpdateState(EnemyController controller)
    {
        EnemySkillType skill = enemy.GetCurrentSkillType();
        string animName = enemy.GetSkillTriggerName(skill);
        stateInfo = controller.animator.GetCurrentAnimatorStateInfo(0);
        skillEnd = false;

        switch (controller.Enemy.skillA)
        { 
            case EnemySkillType.Dash:
                skillEnd = stateInfo.IsName("Skill_Dash") && stateInfo.normalizedTime >= 0.99f;
                break;
            case EnemySkillType.SpreadShot:
                skillEnd = stateInfo.IsName("Skill_SpreadShot") && stateInfo.normalizedTime >= 0.99f;
                break;
                //다른 스킬
        }
        Debug.Log($" IsName(\"Skill_Dash\") = {stateInfo.IsName("Skill_Dash")}, normalizedTime = {stateInfo.normalizedTime}, " + skillEnd);
        if (skillEnd)
        {
            if (controller.Enemy.IsBoss)
            {
                switch (enemy.GetCurrentSkillType())
                {
                    case EnemySkillType.Dash:
                        controller.Enemy.SkillDash();
                        Debug.Log("asdfend");
                        break;
                    case EnemySkillType.SpreadShot:
                        break;
                }
            }
            else
            {
                // 엘리트 스킬 실행
                switch (controller.Enemy.skillA)
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
