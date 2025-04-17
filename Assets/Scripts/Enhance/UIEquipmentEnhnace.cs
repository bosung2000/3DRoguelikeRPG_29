using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIEquipmentEnhance : MonoBehaviour
{
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI enhancementLevelText;
    public TextMeshProUGUI successRateText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI startPreviewText;


    public Image itemIcon;
    public Button enhanceButton;

    public EquipmentEnhancer enhancer;
    public PlayerManager playerManager;

    private ItemData currentEquipment;

   

    private void Start()
    {
        enhanceButton.onClick.AddListener(OnEnhanceButtonClicked);
    }

    public void SetEquipment(ItemData equipment)
    {
        currentEquipment = equipment;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (currentEquipment == null || playerManager == null) return;

        itemNameText.text = currentEquipment.itemName;
        enhancementLevelText.text = $"+{currentEquipment.enhancementLevel}";

        float rate = enhancer.GetSuccessRate(currentEquipment.enhancementLevel);
        float cost = currentEquipment.enhancementCost * Mathf.Pow(currentEquipment.enhancementCostMultiplier, currentEquipment.enhancementLevel);
        int gold = playerManager.Currency.currencies[CurrencyType.Gold];

        successRateText.text = $"성공 확률: {(int)(rate * 100)}%";
        costText.text = $"비용: {(int)cost} 골드";
        goldText.text = $"보유 골드: {gold}";

        itemIcon.sprite = currentEquipment.Icon;

        startPreviewText.text = BuildStatPreviewText(currentEquipment); 
    }

    private void OnEnhanceButtonClicked()
    {
        if (currentEquipment == null || enhancer == null || playerManager == null) return;

        bool success = enhancer.Enhance(currentEquipment, playerManager);

        resultText.text = success ? "강화 성공!" : "강화 실패";
        UpdateUI();
    }

    private string BuildStatPreviewText(ItemData item)
    {
        if (item == null || item.options == null) return "";

        string result = "";
        int level = item.enhancementLevel;

        foreach (var option in item.options)
        {
            float current = option.GetValueWithLevel(level);
            float next = option.GetValueWithLevel(level + 1);
            result += $"{option.type}: {current:F1} → {next:F1}\n";
        }

        return result;
    }


}
