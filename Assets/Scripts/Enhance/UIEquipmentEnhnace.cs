using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquipmentEnhanceUI : MonoBehaviour
{
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI enhancementLevelText;
    public TextMeshProUGUI successRateText;
    public TextMeshProUGUI costText;
    public Image itemIcon;
    public Button enhanceButton;
    public TextMeshProUGUI resultText;

    public EquipmentEnhancer enhancer;
    public Player player;
    private ItemData currentEquipment;
    public PlayerManager playerManager;

    void Start()
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
        if (currentEquipment == null) return;

        itemNameText.text = currentEquipment.itemName;
        enhancementLevelText.text = $"+{currentEquipment.enhancementLevel}";
        successRateText.text = $"성공 확률: {enhancer.successRate * 100}%";

        float currentCost = currentEquipment.enhancementCost * Mathf.Pow(currentEquipment.enhancementCostMultiplier, currentEquipment.enhancementLevel);
        costText.text = $"비용: {(int)currentCost} 골드";

        itemIcon.sprite = currentEquipment.Icon;
    }

    private void OnEnhanceButtonClicked()
    {
        if (currentEquipment == null || player == null || enhancer == null)
        {
            Debug.LogWarning("필수 요소가 세팅되지 않았습니다.");
            return;
        }

        bool success = enhancer.Enhance(currentEquipment, playerManager);

        if (success)
        {
            resultText.text = "강화 성공!";
            resultText.color = Color.green;
        }
        else
        {
            resultText.text = "강화 실패...";
            resultText.color = Color.red;
        }

        UpdateUI(); //강화레벨변경반영
    }
}
