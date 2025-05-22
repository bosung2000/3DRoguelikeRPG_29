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
    Hit,
    Skill,
    Dead
}
public class EnemyController : MonoBehaviour
{
    private Enemy _enemy;
    public Enemy Enemy => _enemy;

    public IEnemyState _currentState;
    public EnemyStateType CurrentStateType { get; private set; }
    public NavMeshAgent agent { get; private set; }
    public Animator animator { get; private set; }
    public Vector3 SpawnPosition { get; private set; }
    public float lastAttackTime { get; private set; }
    public EnemyStateType LastStateType { get; private set; } = EnemyStateType.Idle;
    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (_enemy == null)
        {
            return;
        }

        if (_enemy.Stat == null)
        {
            return;
        }

        SpawnPosition = transform.position;// 스폰 위치 저장

        agent.speed = GetSpeed();
        ChageState(EnemyStateType.Idle);
    }

    private void Update()
    {
        _currentState?.UpdateState(this);
    }

    public void ChageState(EnemyStateType newStateType)
    {
        if (CurrentStateType == newStateType)
        {
            return;
        }

        LastStateType = CurrentStateType;

        _currentState?.ExitState(this);
        _currentState = CreateStateByType(newStateType);
        _currentState?.EnterState(this);

        CurrentStateType = newStateType;
        //Debug.Log("현재" + CurrentStateType);
    }

    public float GetSpeed() => _enemy.Stat.GetStatValue(EnemyStatType.Speed);//속도를 가져옴
    public float GetAttack() => _enemy.Stat.GetStatValue(EnemyStatType.Attack);//공격력을 가져옴
    public float GetHP() => _enemy.Stat.GetStatValue(EnemyStatType.MaxHP);//최대 체력을 가져옴
    public float GetCurrentHP() => _enemy.Stat.GetStatValue(EnemyStatType.HP);//현재 체력을 가져옴
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
            EnemyStateType.Hit => new EnemyHitState(),
            EnemyStateType.Skill => new EnemySkillState(),
            _ => null
        };
    }

    public void ResetAttackCooldown()
    {
        lastAttackTime = Time.time;
    }


    /// <summary>
    /// 디버깅용 함수들
    /// </summary>
    public void OnDrawGizmos()
    {
        if (_enemy == null || _enemy.Stat == null) return;

        Gizmos.color = Color.red;

        //추적범위
        float chaseRange = _enemy.Stat.GetStatValue(EnemyStatType.ChaseRange);
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        //추적범위 이탈
        Gizmos.color = new Color(255f / 255f, 0f / 255f, 221f / 255);
        Gizmos.DrawWireSphere(transform.position, chaseRange * 1.5f);
        //공격범위
        float attackRange = _enemy.Stat.GetStatValue(EnemyStatType.AttackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);

    }
}