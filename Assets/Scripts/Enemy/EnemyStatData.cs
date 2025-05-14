using UnityEngine;

public enum EnemyType
{
    Normal,
    Elite,
    Boss
}

public enum EnemyRoleType
{
    Melee,  //근접
    Ranged, //원거리
}

public enum EnemySkillType
{
    None,
    Dash,       //돌진
    SpreadShot, //투사체 다중 발사

}

[CreateAssetMenu(fileName = "EnemyStatData", menuName = "Enemy/EnemyStat")]
public class EnemyStatData : ScriptableObject
{
    [Header("기본 정보")]
    [SerializeField] private int _index;
    [SerializeField] private string _enemyName;
    [SerializeField] private EnemyType _enemyType;
    [SerializeField] private EnemyRoleType _enemyRole;

    [Header("스킬 정보")]
    [SerializeField] private float _skillCooldown;
    [SerializeField] private EnemySkillType _skillA = EnemySkillType.None;
    [SerializeField] private EnemySkillType _skillB = EnemySkillType.None;

    [Header("스탯 정보")]
    [SerializeField] private int _maxHP;
    [SerializeField] private int _HP;
    [SerializeField] private int _attack;
    [SerializeField] private int _speed;
    [SerializeField] private int _gold;
    [SerializeField] private int _soul;

    [Header("전투 정보")]
    [SerializeField] private float _attackRange;    //공격범위
    [SerializeField] private float _attackCooldown; //공격 쿨

    [Header("탐지 정보")]
    [SerializeField] private float _chaseRange; //탐지 범위


    public int Index => _index;
    public string EnemyName => _enemyName;
    public EnemyType EnemyType => _enemyType;
    public EnemyRoleType EnemyRole => _enemyRole;
    public int MaxHP => _maxHP;
    public int HP => _HP;
    public int Attack => _attack;
    public int Speed => _speed;
    public int Gold => _gold;
    public int Soul => _soul;
    public float AttackRange => _attackRange;
    public float AttackCooldown => _attackCooldown;
    public float ChaseRange => _chaseRange;
    public float SkillCooldown => _skillCooldown;
    public EnemySkillType SkillA => _skillA;
    public EnemySkillType SkillB => _skillB;
}