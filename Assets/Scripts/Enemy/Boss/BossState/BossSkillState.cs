using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BossSkillType
{
    Dash,
    SlamWave
}
public class BossSkillState : IBossState
{
    private BossSkillType _skillType;
    //공용 변수
    private Transform _target;
    private bool _isActionDone = false;

    //돌진용
    private float _dashSpeed = 10f;
    private float _dashDistance = 8f;
    private Vector3 _dashStartPos;

    public BossSkillState(BossSkillType skillType)
    {
        _skillType = skillType;
    }
    public void EnterState(BossController controller)
    {
        Boss boss = controller.GetComponent<Boss>();
        _target = GameObject.FindGameObjectWithTag("Player").transform;

        _isActionDone = false;

        controller.agent.isStopped = true;

        switch(_skillType)
        {
            case BossSkillType.Dash:
                StartDach(controller);
                break;
                case BossSkillType.SlamWave:
                StartSlamWave(controller);
                break;
        }
    }

    public void ExitState(BossController controller)
    {
        controller.animator.ResetTrigger("Skill1_Melee");
        controller.animator.ResetTrigger("Skill2_Melee");
    }

    public void UpdateState(BossController controller)
    {
        switch (_skillType)
        {
            case BossSkillType.Dash:
                UpdateDash(controller);
                break;
            case BossSkillType.SlamWave:
                UpdateSlamWave(controller);
                break;
        }
    }

    private void  StartDach(BossController controller)
    {

    }
    private void UpdateDash(BossController controller)
    {

    }
    private void StartSlamWave(BossController controller)
    {

    }
    private void UpdateSlamWave(BossController controller)
    {

    }
}
