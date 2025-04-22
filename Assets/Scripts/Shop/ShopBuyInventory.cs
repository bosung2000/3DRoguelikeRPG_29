using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopBuyInventory : MonoBehaviour
{
    
    [SerializeField] private Transform SlotParent;
    [SerializeField] private ShopBuySlot slotPrefab;
    private List<ShopBuySlot> ShopBuyslots;
    private Shop shop;
    

    public void Initialize(Shop _shop)
    {
        shop = _shop;
        shop.ShopitemChange += ShowShopItems;        
        ShowShopItems();
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
            slot.SetSlotData_Equip(item);
            slot.OnitemClicked += OnShopItemSelected;
            ShopBuyslots.Add(slot);
        }
    }
    

    private void RemoveSlots()
    {
        if (ShopBuyslots == null)
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
        purchasePopup.Initialize(shop,item);
        

    }
}
