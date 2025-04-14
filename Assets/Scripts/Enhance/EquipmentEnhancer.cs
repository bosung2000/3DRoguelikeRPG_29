using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentEnhancer : MonoBehaviour
{
    [Range(0f, 1f)]
    public float successRate = 0.9f;

    public bool Enhance(ItemData equipment, PlayerManager playerManager)
    {
        if (equipment == null)
        {
            Debug.Log("강화할 아이템이 없습니다.");
            return false;
        }

        if (equipment.itemType != ItemType.Equipment)
        {
            Debug.Log("해당 아이템은 장비가 아닙니다.");
            return false;
        }

        if (equipment.enhancementLevel >= equipment.maxEnhancementLevel)
        {
            Debug.Log("이미 최대 강화 레벨에 도달했습니다.");
            return false;
        }

        float currentCost = equipment.enhancementCost * Mathf.Pow(equipment.enhancementCostMultiplier, equipment.enhancementLevel);
        Debug.Log($"현재 강화 비용: {currentCost}");

        //PlayerManager를 통해 골드 확인
        int playerGold = playerManager.Currency.currencies[CurrencyType.Gold];
        if (playerGold < currentCost)
        {
            Debug.Log("골드가 부족합니다.");
            return false;
        }

        //골드차감
        bool deducted = playerManager.Currency.AddCurrency(CurrencyType.Gold, -(int)currentCost);
        if (!deducted)
        {
            Debug.Log("골드 차감 실패");
            return false;
        }

        Debug.Log($"{(int)currentCost} 골드를 차감하였습니다. 남은 골드: {playerManager.Currency.currencies[CurrencyType.Gold]}");

        if (Random.value <= successRate)
        {
            equipment.enhancementLevel++;
            Debug.Log($"강화 성공! 강화 레벨: {equipment.enhancementLevel}");
            return true;
        }
        else
        {
            Debug.Log("강화 실패.");
            return false;
        }
    }

}

