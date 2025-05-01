using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.LowLevel;

public enum BossStateType
{
    None,
    KeepDistance,
    Attack,
    Skill,
    Phase2,
    Dead
}

public enum BossRoleType
{
    Melee,
    Ranged,
    Hybrid
}
public interface IBossState
{ 
    void EnterState(BossController controller);
    void UpdateState(BossController controller);
    void ExitState(BossController controller);
}

public class BossController : MonoBehaviour
{
    private IBossState _currentState;
    public BossStateType CurrentStateType { get; private set; }
    public Animator animator {  get; private set; }
    public NavMeshAgent agent { get; private set; }
    public Transform Target { get; private set; }


    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        Target = GameObject.FindGameObjectWithTag("Player")?.transform;
    }
    void Start()
    {
        ChageState(BossStateType.KeepDistance);
    }

    void Update()
    {
        _currentState?.UpdateState(this);
    }

    public void ChageState(BossStateType newState)
    {
        if (CurrentStateType == newState) return;

        Debug.Log($"보스 상태 전환: {CurrentStateType} → {newState}");

        _currentState?.ExitState(this);
        _currentState = CreateState(newState);
        _currentState?.EnterState(this);
        CurrentStateType = newState;
    }

    private IBossState CreateState(BossStateType newState)
    {
        return newState switch
        {
            //상태 추가마다 넣기
            BossStateType.KeepDistance => new BossKeepDistanceState(),
            BossStateType.Attack => new BossAttackState(),
            BossStateType.Skill => new BossSkillState(),
            BossStateType.Phase2 => new BossPhaseState(),
            BossStateType.Dead => new BossDeadState(),
            _ => null

        };
    }
}
