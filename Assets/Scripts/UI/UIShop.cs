using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIShop : PopupUI
{
    //판매 나의인벤토리 
    //구매 shop가지고 있는 아이템 
    //판매해야할 아이템 list 가지고있고 
    //buy > inventory읽어와서 show 
    //sell > uishop(판매list)이중에서 색출해서(뭐 등급이 맞게 높은 등급일수록 좋은아이템이 나와야하잔아)
    // 그럼 아아템의 등급이 있어야하겠는데 > tier            
    [SerializeField] TextMeshProUGUI goldTxt;


    [SerializeField] private Transform SlotParent;
    [SerializeField] private ShopBuySlot slotPrefab;
    private List<ShopBuySlot> ShopBuyslots;
    private Shop shop;
    private PlayerManager playerManager;


    public void Initialize(Shop _shop,PlayerManager _playerManager)
    {
        shop = _shop;
        playerManager = _playerManager;
        shop.ShopitemChange += ShowShopItems;
        shop.ShopitemChange += ShowShopGold;
        ShowShopItems();
        ShowShopGold();
    }
    //UI 다시 보여주는 로직 만들어야됨 >변경되는곳에 이벤트를 달아주자 slotdata가 
    public void ShowShopItems()
    {
        RemoveSlots();
        ShopBuyslots = new List<ShopBuySlot>();
        var availableItems = shop.GetAvailableItems();

        foreach (var item in availableItems)
        {
            var slot = Instantiate(slotPrefab, SlotParent);
            slot.SetSlotData(item);
            slot.OnitemClicked += OnShopItemSelected;
            ShopBuyslots.Add(slot);
        }
    }
    public void ShowShopGold()
    {
        goldTxt.text = playerManager.Currency.currencies[CurrencyType.Gold].ToString();
    }

    private void RemoveSlots()
    {
        if (ShopBuyslots ==null)
        {
            return;
        }
        // 모든 슬롯의 이벤트 구독 해제
        foreach (var slot in ShopBuyslots)
        {
            if (slot != null)
            {
                slot.OnitemClicked -= OnShopItemSelected;
            }
        }

        // 모든 슬롯 오브젝트 파괴
        foreach (var slot in ShopBuyslots)
        {
            if (slot != null)
            {
                Destroy(slot.gameObject);
            }
        }

    }

    private void OnShopItemSelected(SlotItemData item)
    {
        //아이템 정보를 보여주는 창 1개 띄운다. > 구매 창 

        var purchasePopup = UIManager.Instance.ShowPopupUI<UIPurchasePopup>();
        purchasePopup.Initialize(shop);
        purchasePopup.Show(item);

    }
    private void ResetSlots()
    {
        foreach (var slot in ShopBuyslots)
        {
            slot.ClearSlot();
        }
    }
}
