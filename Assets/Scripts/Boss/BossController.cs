using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum BossStateType
{
    Idle,
    Chase,
    Attack,
    Skill1,
    Skill2,
    Phase2,
    Dead
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

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }
    void Start()
    {
        ChageState(BossStateType.Idle);
    }

    void Update()
    {
        _currentState?.UpdateState(this);
    }

    public void ChageState(BossStateType newState)
    {
        if (CurrentStateType == newState) return;

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
            //BossStateType.Idle => new BossIdleState(),


        };
    }
}
