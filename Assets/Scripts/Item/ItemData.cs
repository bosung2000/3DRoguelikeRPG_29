using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 아이템 기본 타입
public enum ItemType
{
    Equipment,    // 장비
    Relics,       // 유물 
    Consumable,   // 소모품
    Quest,        // 퀘스트 아이템
    Material      // 재료
}

public enum EquipType
{
    None,         // 장비 아님
    Weapon,
    Coat,
    Shoes,
    Glove,
    Relics
}

public enum UseType
{
    None,         // 소모품 아님
    HpPotion,
    MpPotion,
    StatBoost,    // 일시적 스텟 증가
    Buff          // 지속 버프
}

public enum ConditionType
{
    Power,//int 공격력
    MaxMana,//int 최대마나
    Mana,//int 마나
    MaxHealth,//int 최대체력
    Health,//int 체력
    Speed,//int moveSpeed
    reduction,//float 받는피해 감소
    CriticalChance,//float 크리티컬 확률
    CriticalDamage,//float 크리티컬 데미지

    // 유물 능력치 
    absorp,//Float 흡혈량 
    DMGIncrease,//Float 데미지 증가량 
    HPRecovery,//int HP 회복
    MPRecovery, //int MP 회복 
    GoldAcquisition,//Float Glod획득량 증가,
    SkillColltime,//Float 스킬 쿨타임,
    AttackSpeed,//Float 공격속도
}

[Serializable]
public class ItemOption
{
    public ConditionType type;
    public float baseValue;
    public float increasePerLevel; //유뮬은 increasPerLevel 을 0으로 설정 

    public float GetValueWithLevel(int level)
    {
        return baseValue + (increasePerLevel * level);
    }
}

[Serializable]
public class ConsumableEffect
{
    public ConditionType type;
    public float value;           // 회복량 또는 효과 수치
    public float duration;        // 지속 시간 (0이면 즉시 효과)
}

public class ItemData : ScriptableObject
{
    [Header("기본 정보")]
    public int id;
    public ItemType itemType;     // 아이템 기본 타입
    public EquipType equipType;
    public UseType useType;
    public string itemName;
    public string description;
    [Range(1, 10)]
    public int Tier;

    [Header("장비 옵션")]
    public int enhancementLevel = 0;
    public int maxEnhancementLevel = 10;
    public List<ItemOption> options = new List<ItemOption>();

    [Header("소모품 효과")]
    public List<ConsumableEffect> consumableEffects = new List<ConsumableEffect>();
    public int maxStack = 99;     // 최대 중첩 개수

    [Header("경제")]
    public int gold;
    public int enhancementCost = 100;
    public float enhancementCostMultiplier = 1.5f;

    [Header("시각 효과")]
    public Sprite Icon;
    public GameObject itemObj;

    /// <summary>
    /// 장비 옵션 값 얻기
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public float GetOptionValue(ConditionType type)
    {
        foreach (ItemOption option in options)
        {
            if (option.type == type)
            {
                return option.GetValueWithLevel(enhancementLevel);
            }
        }
        return 0f;
    }


    // 아이템 사용 (소모품용)
    public List<ConsumableEffect> Use()
    {
        if (itemType == ItemType.Consumable)
        {
            return consumableEffects;
        }
        return null;
    }

    // 아이템 초기화 (새 아이템 생성 시)
    public void Initialize()
    {
        // 타입에 따라 기본값 설정
        if (itemType == ItemType.Equipment)
        {
            if (useType != UseType.None)
                useType = UseType.None;
        }
        else if (itemType == ItemType.Relics)
        {
            if (useType != UseType.None)
                useType = UseType.None;
            
            if (equipType != EquipType.Relics)
                equipType = EquipType.Relics;
                
            // 유물은 티어 0, 강화 불가능
            Tier = 0;
            maxEnhancementLevel = 0;
            enhancementLevel = 0;
        }
        else if (itemType == ItemType.Consumable)
        {
            if (equipType != EquipType.None)
                equipType = EquipType.None;
        }
    }

    /// <summary>
    /// 아이템의 강화 수치를 초기화합니다. 아이템 판매나 거래 시 호출됩니다.
    /// </summary>
    public void ResetEnhancement()
    {
        // 장비 아이템인 경우에만 강화 수치 초기화
        if (itemType == ItemType.Equipment)
        {
            enhancementLevel = 0;
        }
        // 유물은 이미 enhancementLevel이 0으로 고정되어 있으므로 별도 처리 불필요
        
        // 추가적인 초기화 로직이 필요한 경우 여기에 구현
    }
}
