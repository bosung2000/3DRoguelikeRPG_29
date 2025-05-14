using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;

public enum SkillType
{
    Melee,
    Ranged,
    Buff
}

public enum BuffType
{
    None,
    Heal,
    ATK,
    RES,
}

// 스킬 발사체 타입 추가
public enum ProjectileType
{
    None,
    Normal,     // 일반 발사체
    Penetrating, // 관통 발사체
    Homing,     // 유도 발사체
    Explosive,  // 폭발 발사체
    Multi,      // 다중 발사체
    Chain,      // 체인 발사체
}

// 스킬 시전 패턴 추가
public enum CastPattern
{
    None,
    Single,     // 단일 발사
    Burst,      // 연속 발사
    Spread,     // 부채꼴 발사
    Rain,       // 비처럼 내리는 발사
    Circle      // 원형 발사
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

    [Header("원거리 공격 설정")]
    public ProjectileType projectileType = ProjectileType.Normal; // 발사체 타입
    public CastPattern castPattern = CastPattern.Single; // 시전 패턴
    
    [Header("연속 발사 설정")]
    public int burstCount = 3; // 연속 발사 개수
    public float burstDelay = 0.15f; // 연속 발사 딜레이

    [Header("부채꼴 발사 설정")]
    public int spreadCount = 3; // 부채꼴 발사 개수
    public float spreadAngle = 15f; // 부채꼴 각도

    [Header("비 발사 설정")]
    public int rainCount = 7; // 비 발사 개수
    public float rainRadius = 5f; // 비 발사 범위

    [Header("원형 발사 설정")]
    public int circleCount = 8; // 원형 발사 개수

    [Header("Buff variable")]
    public BuffType buffType;
    
    [Header("오디오")]
    public AudioClip soundEffectPrefab; // 스킬 사운드 이펙트

    // 스킬 생성 시 최대 쿨다운 초기화
    private void OnEnable()
    {
        // maxCooldown이 아직 초기화되지 않았거나 0인 경우,
        // Inspector에서 설정된 cooldown 값을 기본값으로 사용
        if (maxCooldown <= 0)
        {
            maxCooldown = cooldown;
        }
        
        // 스킬 활성화 시 cooldown을 maxCooldown으로 초기화
        // (스킬이 사용 가능한 상태로 시작)
        cooldown = 0;
    }

    // 쿨다운 리셋 메서드
    public void ResetCooldown()
    {
        cooldown = maxCooldown;
    }
    
    // 스킬 레벨업 로직 (예시)
    [Header("레벨업 설정")]
    public int level = 1; // 현재 스킬 레벨
    public int maxLevel = 5; // 최대 스킬 레벨
    
    // 레벨업에 따른 스탯 증가량
    public float valueLevelMultiplier = 1.2f; // 레벨당 데미지 증가 배율
    public float cooldownLevelMultiplier = 0.9f; // 레벨당 쿨다운 감소 배율
    public float rangeLevelMultiplier = 1.1f; // 레벨당 범위 증가 배율
    
    // 스킬 레벨업
    public bool LevelUp()
    {
        if (level >= maxLevel)
            return false;
            
        level++;
        
        // 스탯 업데이트
        value = Mathf.RoundToInt(value * valueLevelMultiplier);
        maxCooldown *= cooldownLevelMultiplier;
        cooldown = maxCooldown;
        attackRange = Mathf.RoundToInt(attackRange * rangeLevelMultiplier);
        
        return true;
    }
}
