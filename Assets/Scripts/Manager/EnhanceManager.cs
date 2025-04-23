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

    private void Update()
    {
        //if(Input.GetKeyDown(KeyCode.U))
        //{
        //    UIManager.Instance.ShowPopupUI<UIEquipmentEnhance>();
        //}
    }
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
            Debug.LogError("장비 또는 플레이어가 null입니다.");
            return false;
        }

        if (equipment.itemType != ItemType.Equipment || equipment.enhancementLevel >= equipment.maxEnhancementLevel)
        {
            Debug.Log("강화할 수 없는 상태입니다.");
            return false;
        }

        float cost = equipment.enhancementCost * Mathf.Pow(equipment.enhancementCostMultiplier, equipment.enhancementLevel);
        int gold = playerManager.Currency.currencies[CurrencyType.Gold];

        if (gold < cost)
        {
            Debug.Log("골드 부족!");
            return false;
        }

        playerManager.Currency.AddCurrency(CurrencyType.Gold, -(int)cost);
        float successRate = GetSuccessRate(equipment.enhancementLevel);

        if (UnityEngine.Random.value <= successRate)
        {
            equipment.enhancementLevel++;
            //강화 성공 부분에 playerstat
            OnSucessEnhancs?.Invoke();
            Debug.Log($"강화 성공! → +{equipment.enhancementLevel}");
            return true;
        }
        else
        {
            Debug.Log("강화 실패...");
            return false;
        }
    }
}
