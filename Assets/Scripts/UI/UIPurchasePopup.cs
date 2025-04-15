using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPurchasePopup : PopupUI
{
    [SerializeField] private Image itemimage;
    [SerializeField] private TextMeshProUGUI power;
    [SerializeField] private TextMeshProUGUI mana;
    [SerializeField] private TextMeshProUGUI health;
    [SerializeField] private TextMeshProUGUI speed;
    [SerializeField] private TextMeshProUGUI Reduction;
    [SerializeField] private TextMeshProUGUI CriticalChance;
    [SerializeField] private TextMeshProUGUI CriticalDamage;
    [SerializeField] private Button btn_Buy;

    SlotItemData currentSlotItem;
    Shop shop;

    private void Awake()
    {
        btn_Buy.onClick.AddListener(OnBuyItem);
        closeButton.onClick.AddListener(ClosePopup);
    }
    public void Initialize(Shop _shop)
    {
        shop = _shop;
    }

    private void OnBuyItem()
    {
        if (currentSlotItem != null)
        {
            //아이템 구매시 골드 차감 +ui update
            if (shop.TryPurchaseItem(currentSlotItem.item))
            {
                //인벤토리 넣기
                GameManager.Instance.InventoryManager.AddInventoryItem(currentSlotItem.item);
                //shoplist에서 삭제
                shop.RemoveShopItemlist(currentSlotItem);
            }

            UIManager.Instance.ClosePopupUI(this);
        }

    }
    private void ClosePopup()
    {
        UIManager.Instance.ClosePopupUI(this);
    }

    public void Show(SlotItemData item)
    {
        currentSlotItem = item;
        base.Show();
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (currentSlotItem == null) return;

        if (currentSlotItem.item.Icon != null)
        {
            itemimage.sprite = currentSlotItem.item.Icon;
        }

        // 능력치 표시
        power.text = $"공격력: {currentSlotItem.item.GetOptionValue(ConditionType.Power)}";
        mana.text = $"마나: {currentSlotItem.item.GetOptionValue(ConditionType.Mana)}";
        health.text = $"체력: {currentSlotItem.item.GetOptionValue(ConditionType.Health)}";
        speed.text = $"속도: {currentSlotItem.item.GetOptionValue(ConditionType.Speed)}";
        Reduction.text = $"피해감소: {currentSlotItem.item.GetOptionValue(ConditionType.reduction)}";
        CriticalChance.text = $"치명타확률: {currentSlotItem.item.GetOptionValue(ConditionType.CriticalChance)}";
        CriticalDamage.text = $"치명타피해: {currentSlotItem.item.GetOptionValue(ConditionType.CriticalDamage)}";
    }
}
