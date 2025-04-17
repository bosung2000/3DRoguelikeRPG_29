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
    public void Initialize(Shop _shop, SlotItemData item)
    {
        shop = _shop;
        currentSlotItem = item;
        base.Show();
        UpdateUI();
    }

    private void OnBuyItem()
    {
        //현재 아이템이 존재하는지  and shoplist에 존재하는지 
        if (currentSlotItem != null && shop.GetAvailableItems().Contains(currentSlotItem))
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
        else
        {
            Debug.Log("구매과정에서 무언가 잘못됬습니다.");
        }
    }
    private void ClosePopup()
    {
        UIManager.Instance.ClosePopupUI(this);
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
