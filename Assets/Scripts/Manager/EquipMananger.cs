using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class EquipMananger : MonoBehaviour
{
    public Dictionary<EquipType, ItemData> EquipDicionary { get; private set; }
    public Dictionary<int, ItemData> RelicsDictionary { get; private set; } // 유물 전용 딕셔너리(id를 키로 사용)
    
    // 최대 장착 가능한 유물 개수
    private const int MAX_RELICS = 3;

    public event Action<EquipType, ItemData, bool> OnEquipedChanged;
    public event Action<ItemData, bool> OnRelicsChanged; // 유물 장착/해제 이벤트

    private void Start()
    {
        init();
    }

    public void init()
    {
        EquipDicionary = new Dictionary<EquipType, ItemData>();
        RelicsDictionary = new Dictionary<int, ItemData>();
    }

    // 일반 장비 아이템 장착
    public bool Equipitem(ItemData itemData)
    {
        // 유물 아이템이면 유물 장착 메서드 호출
        if (itemData.itemType == ItemType.Relics)
        {
            return EquipRelic(itemData);
        }

        // 장착된 아이템이 있는가?
        if (EquipDicionary.TryGetValue(itemData.equipType, out ItemData Equipeditemed))
        {
            // 같은 아이템을 장착할려고 하는것 막아야한다.
            if (itemData.id == Equipeditemed.id)
            {
                return false;
            }
            // 장착된 아이템을 제거 
            UnEquipitem(Equipeditemed);
        }
        
        EquipDicionary.Add(itemData.equipType, itemData);
        // 능력치 더해주고 
        AddStats();
        // 장착 관련 이벤트를 발생
        OnEquipedChange(itemData.equipType, itemData, true);
        return true;
    }

    // 유물 아이템 장착
    public bool EquipRelic(ItemData relicItem)
    {
        // 유물 타입이 아니면 처리하지 않음
        if (relicItem.itemType != ItemType.Relics)
        {
            Debug.LogWarning("유물 타입이 아닌 아이템입니다.");
            return false;
        }

        // 이미 장착된 유물인지 확인
        if (RelicsDictionary.ContainsKey(relicItem.id))
        {
            Debug.Log("이미 장착된 유물입니다.");
            return false;
        }
        
        // 장착된 유물이 최대 개수에 도달했는지 확인
        if (RelicsDictionary.Count >= MAX_RELICS)
        {
            Debug.Log($"유물은 최대 {MAX_RELICS}개까지만 장착할 수 있습니다.");
            return false;
        }

        // 유물 장착
        RelicsDictionary.Add(relicItem.id, relicItem);
        
        // 유물 효과 적용
        AddRelicStats();
        
        // 유물 장착 이벤트 발생
        OnRelicsChanged?.Invoke(relicItem, true);
        
        return true;
    }

    // 일반 장비 아이템 해제
    public bool UnEquipitem(ItemData itemData)
    {
        // 유물 아이템이면 유물 해제 메서드 호출
        if (itemData.itemType == ItemType.Relics)
        {
            return UnEquipRelic(itemData);
        }

        if (EquipDicionary.TryGetValue(itemData.equipType, out ItemData eqipeditemed))
        {
            EquipDicionary.Remove(eqipeditemed.equipType);
            AddStats();
            OnEquipedChange(itemData.equipType, itemData, false);

            return true;
        }
        return false;
    }

    // 유물 아이템 해제
    public bool UnEquipRelic(ItemData relicItem)
    {
        // 유물 타입이 아니면 처리하지 않음
        if (relicItem.itemType != ItemType.Relics)
        {
            Debug.LogWarning("유물 타입이 아닌 아이템입니다.");
            return false;
        }

        // 해당 ID의 유물이 장착되어 있는지 확인
        if (RelicsDictionary.TryGetValue(relicItem.id, out _))
        {
            RelicsDictionary.Remove(relicItem.id);
            
            // 유물 효과 재계산
            AddRelicStats();
            
            // 유물 해제 이벤트 발생
            OnRelicsChanged?.Invoke(relicItem, false);
            
            return true;
        }
        
        return false;
    }

    // 장착된 장비의 스탯 효과 적용
    private void AddStats()
    {
        // 종합 stats값 
        Dictionary<PlayerStatType, float> totalconditionTypes = new Dictionary<PlayerStatType, float>();

        // 모든 장비 스탯 보너스 초기화 (이전 장비 효과 제거)
        PlayerStat playerStat = GameManager.Instance.PlayerManager.Player._playerStat;
        playerStat.ClearEquipmentBonuses();  // 이 메서드는 PlayerStat에 추가해야 함

        // 모든 장착된 아이템에서 스탯 보너스 계산
        foreach (var item in EquipDicionary.Values)
        {
            // 아이템의 모든 옵션을 순회
            foreach (var option in item.options)
            {
                // ConditionType을 PlayerStatType으로 변환
                ConditionType conditionType = option.type;
                PlayerStatType statType = ConvertToPlayerStatType(conditionType);
                float value = option.GetValueWithLevel(item.enhancementLevel);

                //Dictionary에 존재하지 않는 키에 값을 더하려고 하면 KeyNotFoundException 예외가 발생
                //때문에 값이 없다면 0으로 초기화를 해주고 값을 더해주는 방식 
                if (!totalconditionTypes.ContainsKey(statType))
                {
                    totalconditionTypes[statType] = 0f;
                }
                totalconditionTypes[statType] += value;
            }
        }
        playerStat.AddEquipmentBonus(totalconditionTypes);
    }

    // 장착된 유물의 스탯 효과 적용
    private void AddRelicStats()
    {
        Dictionary<PlayerStatType, float> totalRelicBonuses = new Dictionary<PlayerStatType, float>();
        
        // 플레이어 스탯 참조
        PlayerStat playerStat = GameManager.Instance.PlayerManager.Player._playerStat;
        playerStat.ClearRelicBonuses(); // 이 메서드는 PlayerStat에 추가해야 함
        
        // 모든 장착된 유물에서 스탯 보너스 계산
        foreach (var relic in RelicsDictionary.Values)
        {
            foreach (var option in relic.options)
            {
                PlayerStatType statType = ConvertToPlayerStatType(option.type);
                // 유물은 강화가 없으므로 enhancementLevel은 0
                float value = option.GetValueWithLevel(0);
                
                if (!totalRelicBonuses.ContainsKey(statType))
                {
                    totalRelicBonuses[statType] = 0f;
                }
                totalRelicBonuses[statType] += value;
            }
        }
        
        playerStat.AddRelicBonus(totalRelicBonuses);
    }

    // 장착된 모든 아이템(장비+유물)의 스탯 효과 재계산
    public void RecalculateAllStats()
    {
        AddStats();
        AddRelicStats();
    }

    // 장착된 모든 유물 가져오기
    public List<ItemData> GetAllEquippedRelics()
    {
        return new List<ItemData>(RelicsDictionary.Values);
    }
    
    // 현재 장착된 유물 개수 확인
    public int GetEquippedRelicsCount()
    {
        return RelicsDictionary.Count;
    }
    
    // 유물 슬롯이 비어있는지 확인
    public bool HasFreeRelicSlot()
    {
        return RelicsDictionary.Count < MAX_RELICS;
    }

    // 특정 유물이 장착되어 있는지 확인
    public bool IsRelicEquipped(int relicId)
    {
        return RelicsDictionary.ContainsKey(relicId);
    }

    // ConditionType을 PlayerStatType으로 변환하는 유틸리티 메서드
    private PlayerStatType ConvertToPlayerStatType(ConditionType conditionType)
    {
        switch (conditionType)
        {
            case ConditionType.Power:
                return PlayerStatType.Attack;
            case ConditionType.Health:
                return PlayerStatType.MaxHP;
            case ConditionType.Mana:
                return PlayerStatType.MaxMP;
            case ConditionType.Speed:
                return PlayerStatType.MoveSpeed;
            case ConditionType.reduction:
                return PlayerStatType.DMGReduction;
            case ConditionType.CriticalChance:
                return PlayerStatType.CriticalChance;
            case ConditionType.CriticalDamage:
                return PlayerStatType.CriticalDamage;
            case ConditionType.absorp:
                return PlayerStatType.absorp;
            case ConditionType.DMGIncrease:
                return PlayerStatType.DMGIncrease;
            case ConditionType.HPRecovery:
                return PlayerStatType.HPRecovery;
            case ConditionType.MPRecovery:
                return PlayerStatType.MPRecovery;
            case ConditionType.GoldAcquisition:
                return PlayerStatType.GoldAcquisition;
            case ConditionType.SkillColltime:
                return PlayerStatType.SkillColltime;
            case ConditionType.AttackSpeed:
                return PlayerStatType.AttackSpeed;
            // 기타 조건 추가
            default:
                Debug.LogWarning($"Unsupported ConditionType: {conditionType}");
                return PlayerStatType.MaxHP;  // 기본값 반환
        }
    }

    /// <summary>
    /// add = true /remove =false
    /// </summary>
    /// <param name="equipType"></param>
    /// <param name="itemData"></param>
    /// <param name="AddorRemove"></param>
    private void OnEquipedChange(EquipType equipType, ItemData itemData, bool AddorRemove)
    {
        OnEquipedChanged?.Invoke(equipType, itemData, AddorRemove);
    }
}
