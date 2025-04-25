
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;

public enum SkillType
{
    Melee,
    Ranged,
    Buff,
}
public enum BuffType
{
    None,
    Heal,
    ATK,
    RES,
}

[CreateAssetMenu(fileName = "Skill", menuName = "New Skill")]
public class Skill : ScriptableObject
{
    [Header("Info")]
    public int index;
    public string _name;
    public string description;
    public int value;
    public int requiredMana;
    public float cooldown;
    public float maxCooldown; // 최대 쿨다운 (초기화용)
    public int attackRange;
    public int projectileSpeed;
    public SkillType skillType;
    public Sprite icon; // 적 아이콘
    public bool isOwned; //현재 이 스킬을 갖고 있는지
    public GameObject projectilePrefabs; // instantiate 할 투사체 프리팹
    public GameObject effectPrefab; // 스킬 이펙트 프리팹

    [Header("Buff variable")]
    public BuffType buffType;

    // 스킬 생성 시 최대 쿨다운 초기화
    private void OnEnable()
    {
        maxCooldown = cooldown;
    }

    // 쿨다운 리셋 메서드
    public void ResetCooldown()
    {
        cooldown = maxCooldown;
    }
}
