using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BossStatData", menuName = "Enemy/BossData")]
public class BossStatData : ScriptableObject
{
    [Header("기본 정보")]
    [SerializeField] private int _maxHP;
    [SerializeField] private int _HP;
    [SerializeField] private int _attack;
    [SerializeField] private int _speed;
    [SerializeField] private int _gold;
    [SerializeField] private int _soul;

    [Header("공격 정보")]
    [SerializeField] private int _attackRange;
    [SerializeField] private int _attackCooldown;
    [SerializeField] private int _keepDistance;

    [Header("스킬 정보")]
    [SerializeField] private int _skill1Cooldown;
    [SerializeField] private int _skill2Cooldown;

    [Header("페이즈 전환")]
    [SerializeField] private float _phase2HPThreshold;

    public int MaxHP => _maxHP;
    public int HP => _HP;
    public int Attack => _attack;
    public int Speed => _speed;
    public int Gold => _gold;
    public int Soul => _soul;
    public int AttackRange => _attackRange;
    public int AttackCooldown => _attackCooldown;
    public int KeepDistance => _keepDistance;
    public int Skill1Cooldown => _skill1Cooldown;
    public int Skill2Cooldown => _skill2Cooldown;
    public float Phase2HpThreshold => _phase2HPThreshold;
}
