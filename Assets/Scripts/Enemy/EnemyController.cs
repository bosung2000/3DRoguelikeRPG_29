using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public interface IEnemyState
{
    void EnterState(EnemyController controller);
    void UpdateState(EnemyController controller);
    void ExitState(EnemyController controller);
}
public enum EnemyStateType
{
    None,
    Idle,
    Chase,
    Attack,
    Dead
}
public class EnemyController : MonoBehaviour
{
    private Enemy _enemy;
    private IEnemyState _currentState;

    public EnemyStateType CurrentStateType { get; private set; }
    //public Animator animator {  get; private set; }

    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
        //animator = GetComponent<Animator>();
    }

    private void Start()
    {

        if (_enemy == null)
        {
            Debug.LogError("EnemyController: _enemy가 null!");
            return;
        }

        if (_enemy.Stat == null)
        {
            Debug.LogError("EnemyController: _enemy.Stat이 null!");
            return;
        }
        ChageState(EnemyStateType.Idle);
    }

    private void Update()
    {
        _currentState?.UpdateState(this);
    }

    public void ChageState(EnemyStateType newStateType)
    {
        Debug.Log($"[ChageState] 현재 상태 : {CurrentStateType}");
        Debug.Log($"[ChageState] 상태 전이 요청: {newStateType}");

        if (CurrentStateType == newStateType)
        {
            Debug.Log("[ChageState] 같은 상태로 전이 시도 → 무시");
            return;
        }

        _currentState?.ExitState(this);
        _currentState = CreateStateByType(newStateType);
        if (_currentState == null)
        {
            Debug.LogError("[ChageState] 상태 생성 실패!");
            return;
        }

        Debug.Log("[ChageState] 상태 객체 생성 성공. EnterState 호출 시도");
        _currentState?.EnterState(this);

        CurrentStateType = newStateType;
        Debug.Log("[ChageState] 상태 전이 완료");

    }
    
    public float GetSpeed() => _enemy.Stat.GetStatValue(EnemyStatType.Speed);//속도를 가져옴
    public float GetAttack() => _enemy.Stat.GetStatValue(EnemyStatType.Attack);//공격력을 가져옴
    public float GetHP() => _enemy.Stat.GetStatValue(EnemyStatType.MaxHP);//최대 체력을 가져옴
    public float GetStat(EnemyStatType type) => _enemy.Stat.GetStatValue(type);//그외의 스탯을 가져옴
    public Transform GetTarget()//타겟위치 정보를 가져옴
    {
        return _enemy.GetPlayerTarget();
    }

    private IEnemyState CreateStateByType(EnemyStateType type)
    {
        return type switch
        {
            EnemyStateType.Idle => new EnemyIdleState(),
            EnemyStateType.Chase => new EnemyChaseState(),
            EnemyStateType.Attack => new EnemyAttackState(),
            EnemyStateType.Dead => new EnemyDeadState(),
            _ => null
        };
    }
}
