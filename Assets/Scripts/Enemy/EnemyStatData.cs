using System.Collections;
using System.Collections.Generic;
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
    Support //지원
}

[CreateAssetMenu(fileName = "EnemyStatData", menuName = "Enemy/EnemyStat")]
public class EnemyStatData : ScriptableObject
{
    [Header("기본 정보")]
    [SerializeField] private int _index;
    [SerializeField] private string _enemyName;
    [SerializeField] private EnemyType _enemyType;
    [SerializeField] private EnemyRoleType _enemyRole;

    [Header("스탯 정보")]
    [SerializeField] private int _maxHP;
    [SerializeField] private int _HP;
    [SerializeField] private int _attack;
    [SerializeField] private int _speed;
    [SerializeField] private int _currency;

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
    public int Currency => _currency;
    public float AttackRange => _attackRange;
    public float AttackCooldown => _attackCooldown;
    public float ChaseRange => _chaseRange;
}
