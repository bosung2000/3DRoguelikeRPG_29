using UnityEngine;
using UnityEngine.AI;

public class EnemySkillState : IEnemyState
{
    Enemy enemy;
    public bool skillEnd;
    AnimatorStateInfo stateInfo;

    private bool transitionedToAttack = false;
    private bool shockWaveTriggered = false;
    private bool jumpedForward = false;

    //점프 스킬관련 변수
    private bool isJumping = false;
    private float jumpElapsed = 0f;
    private float jumpDuration = 0.5f;
    private Vector3 jumpStartPos;
    private Vector3 jumpTargetPos;
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
            enemy.CurrentSkillChoice = Random.Range(0, 2);
            
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
            controller.Enemy.ResetSkillCooldown();
        }

        if (controller.agent != null && controller.agent.enabled && controller.agent.isOnNavMesh)
        {
            controller.agent.isStopped = false;
        }

        isJumping = false;
    }

    public void UpdateState(EnemyController controller)
    {
        EnemySkillType skill = enemy.GetCurrentSkillType();
        stateInfo = controller.animator.GetCurrentAnimatorStateInfo(0);
        skillEnd = false;

        //충격파 관련 코드
        if (skill == EnemySkillType.ShockWave)
        {
            // 점프 이동 시작
            if (!jumpedForward && stateInfo.IsName("Skill_ShockWave_Jump"))
            {
                Transform player = enemy.GetPlayerTarget();
                Vector3 toPlayer = (player.position - enemy.transform.position).normalized;
                toPlayer.y = 0f;
                float offset = 1.5f;

                Vector3 target = player.position - toPlayer * offset;
                if (NavMesh.SamplePosition(target, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
                    target = hit.position;

                jumpStartPos = enemy.transform.position;
                jumpTargetPos = target;
                jumpElapsed = 0f;
                isJumping = true;

                jumpedForward = true;
            }

            // 점프 이동 처리
            if (isJumping)
            {
                jumpElapsed += Time.deltaTime;
                float t = Mathf.Clamp01(jumpElapsed / jumpDuration);
                float height = 2.5f;
                float yOffset = Mathf.Sin(Mathf.PI * t) * height;

                Vector3 horizontal = Vector3.Lerp(jumpStartPos, jumpTargetPos, t);
                enemy.transform.position = horizontal + Vector3.up * yOffset;

                if (t >= 1f)
                {
                    isJumping = false;
                    if (controller.agent.enabled && NavMesh.SamplePosition(enemy.transform.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
                    {
                        controller.agent.Warp(hit.position);
                        controller.agent.isStopped = true;
                    }
                }
            }

            // 공격 애니 전환
            if (!transitionedToAttack && stateInfo.IsName("Skill_ShockWave_Jump") && stateInfo.normalizedTime >= 0.15f)
            {
                controller.animator.SetTrigger("JumpToAttack");
                transitionedToAttack = true;
            }

            // 충격파 실행
            if (!shockWaveTriggered && stateInfo.IsName("Skill_ShockWave_Attack") && stateInfo.normalizedTime >= 0.5f)
            {
                enemy.SkillShockWave();
                shockWaveTriggered = true;
            }

            // 종료
            if (stateInfo.IsName("Skill_ShockWave_Attack") && stateInfo.normalizedTime >= 0.99f)
            {
                transitionedToAttack = false;
                shockWaveTriggered = false;
                jumpedForward = false;
                isJumping = false;

                enemy.ResetSkillCooldown();
                controller.ChageState(EnemyStateType.Chase);
            }

            return;
        }

        switch(skill)
        {
            case EnemySkillType.Dash:
                if(stateInfo.IsName("Skill_Dash"))
                {
                    Vector3 playerPos = enemy.GetPlayerTarget().position;
                    float dashProgress = Mathf.Clamp01(stateInfo.normalizedTime / 0.99f);
                    enemy.CreateOrUpdateDashLine(playerPos, 7f, dashProgress);

                    Vector3 toPlayer = playerPos - enemy.transform.position;
                    toPlayer.y = 0f;

                    if (toPlayer != Vector3.zero)
                    {
                        Quaternion lookRot = Quaternion.LookRotation(toPlayer);
                        enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, lookRot, Time.deltaTime * 10f);
                    }
                    if (stateInfo.normalizedTime >= 0.99f)
                    {
                        enemy.DestroyDashLine();
                        break;
                    }
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
