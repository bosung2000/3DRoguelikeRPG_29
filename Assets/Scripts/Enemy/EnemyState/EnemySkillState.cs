using UnityEngine;

public class EnemySkillState : IEnemyState
{
    Enemy enemy;
    public bool skillEnd;
    AnimatorStateInfo stateInfo;

    private bool transitionedToAttack = false;
    private bool shockWaveTriggered = false;
    private bool jumpedForward = false;
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
        }

        if (controller.agent != null && controller.agent.enabled && controller.agent.isOnNavMesh)
        {
            controller.agent.isStopped = false;
        }
    }

    public void UpdateState(EnemyController controller)
    {
        EnemySkillType skill = enemy.GetCurrentSkillType();
        string animName = enemy.GetSkillTriggerName(skill);
        stateInfo = controller.animator.GetCurrentAnimatorStateInfo(0);
        skillEnd = false;

        if (skill == EnemySkillType.ShockWave)
        {
            // 점프 중간 → 공격 애니 전환 트리거
            if (!transitionedToAttack && stateInfo.IsName("Skill_ShockWave_Jump") && stateInfo.normalizedTime >= 0.5f)
            {
                controller.animator.SetTrigger("JumpToAttack");
                transitionedToAttack = true;
            }

            if (!jumpedForward && stateInfo.IsName("Skill_ShockWave_Jump") && stateInfo.normalizedTime >= 0.4f)
            {
                enemy.DoShockWaveJumpMove(); 
                jumpedForward = true;
            }

            // 공격 애니 중 착지 타이밍에 충격파 실행
            if (!shockWaveTriggered && stateInfo.IsName("Skill_ShockWave_Attack") && stateInfo.normalizedTime >= 0.5f)
            {
                enemy.SkillShockWave();
                shockWaveTriggered = true;
            }

            // 애니메이션 종료 후 상태 전환
            if (stateInfo.IsName("Skill_ShockWave_Attack") && stateInfo.normalizedTime >= 0.99f)
            {
                transitionedToAttack = false;
                shockWaveTriggered = false;
                enemy.ResetSkillCooldown();
                controller.ChageState(EnemyStateType.Chase);
            }

            return;
        }

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

        if (skillEnd)
        {
            if (controller.Enemy.IsBoss)
            {
                switch (enemy.GetCurrentSkillType())
                {
                    case EnemySkillType.Dash:
                        controller.Enemy.SkillDash();
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
