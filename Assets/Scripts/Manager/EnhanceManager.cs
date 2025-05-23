using System;
using System.Collections.Generic;
using UnityEngine;

public class EnhanceManager : MonoBehaviour
{
    [Header("강화 확률 설정")]
    [SerializeField] private List<float> enhancementSuccessRates;
    [SerializeField] private float minimumSuccessRate = 0.05f;
    [SerializeField] private float fallbackRateDropPerLevel = 0.1f;

    public event Action OnSucessEnhancs;


    public float GetSuccessRate(int level)
    {
        if (level < enhancementSuccessRates.Count)
        {
            return enhancementSuccessRates[level];
        }
        else
        {
            float rate = 1f - (level * fallbackRateDropPerLevel);
            return Mathf.Max(rate, minimumSuccessRate);
        }
    }

    public bool Enhance(ItemData equipment, PlayerManager playerManager)
    {
        if (equipment == null || playerManager == null)
        {        
            return false;
        }

        if (equipment.itemType != ItemType.Equipment || equipment.enhancementLevel >= equipment.maxEnhancementLevel)
        {          
            return false;
        }

        float cost = equipment.enhancementCost * Mathf.Pow(equipment.enhancementCostMultiplier, equipment.enhancementLevel);
        int gold = playerManager.Currency.currencies[CurrencyType.Gold];

        if (gold < cost)
        {           
            return false;
        }

        playerManager.Currency.AddCurrency(CurrencyType.Gold, -(int)cost);
        float successRate = GetSuccessRate(equipment.enhancementLevel);

        if (UnityEngine.Random.value <= successRate)
        {
            equipment.enhancementLevel++;
            //강화 성공 부분에 playerstat
            if (GameManager.Instance.EquipMananger.EquipDicionary.TryGetValue(equipment.equipType,out ItemData Equipeditem))
            {
                if (Equipeditem.id ==equipment.id)
                {
                    //강화된 아이템이 장착된 아이템이라는뜻 
                    GameManager.Instance.EquipMananger.RecalculateAllStats();
                }
            }
            OnSucessEnhancs?.Invoke();
            
            return true;
        }
        else
        {        
            return false;
        }
    }
}
