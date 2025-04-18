using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISellPopup : PopupUI
{
    [SerializeField] private Image itemimage;
    [SerializeField] private TextMeshProUGUI power;
    [SerializeField] private TextMeshProUGUI mana;
    [SerializeField] private TextMeshProUGUI health;
    [SerializeField] private TextMeshProUGUI speed;
    [SerializeField] private TextMeshProUGUI Reduction;
    [SerializeField] private TextMeshProUGUI CriticalChance;
    [SerializeField] private TextMeshProUGUI CriticalDamage;
    [SerializeField] private TextMeshProUGUI Price;
    [SerializeField] private Button btn_Sell;


    SlotItemData currentSlotItem;
    InventoryManager inventoryManager;
    PlayerManager playerManager;
    ShopSellInventory shopSellInventory;
    UIShop uIShop;

    private void Awake()
    {
        btn_Sell.onClick.AddListener(OnSellItem);
        closeButton.onClick.AddListener(ClosePopup);
        
    }
    private void Start()
    {
        playerManager = GameManager.Instance.PlayerManager;
    }
    public void Initialize(SlotItemData item, InventoryManager _inventoryManager, ShopSellInventory _shopSellInventory, UIShop _uIShop)
    {
        currentSlotItem = item;
        inventoryManager = _inventoryManager;
        shopSellInventory = _shopSellInventory;
        uIShop = _uIShop;
        base.Show();
        UpdateUI();
    }

    private void OnSellItem()
    {
        //현재 아이템이 존재하는지  and 인벤토리에 존재하는지 
        if (currentSlotItem != null && inventoryManager.slotItemDatas.Contains(currentSlotItem))
        {
            //골드 차감해주고 
            if (playerManager.Currency.AddCurrency(CurrencyType.Gold, currentSlotItem.item.gold))
            {
                //slotitemDatas에 데이터를 삭제해주기만 하면 이벤트로 연결되어있어서 
                inventoryManager.RemoveInventoryitme(currentSlotItem.item);
                //골드 UI 변경 
                uIShop.ShowShopGold();
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
        Price.text = $"가격 : {currentSlotItem.item.gold}";
    }
}
