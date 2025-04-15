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
    [SerializeField] GameObject completePopup;
    [SerializeField] Button completeBtn;
    [SerializeField] TextMeshProUGUI completeTxt;
    [SerializeField] Button exitBtn;


    [SerializeField] private Transform itemSlotParent;
    [SerializeField] private UISlot slotPrefab;
    private List<UISlot> slots = new List<UISlot>();
    private Shop shop;

    private void Awake()
    {
        //shop = GameManager.Instance.Shop;
    }

    public void ShowShopItems()
    {
        //ClearSlots();
        var availableItems = shop.GetAvailableItems();

        foreach (var item in availableItems)
        {
            var slot = Instantiate(slotPrefab, itemSlotParent);
            slot.SetSlotData(item);
            slot.OnItemClicked += OnShopItemSelected;
            slots.Add(slot);
        }
    }

    private void OnShopItemSelected(SlotItemData item)
    {
        //아이템 정보를 보여주는 창 1개 띄운다. > 구매 창 

        var purchasePopup = UIManager.Instance.ShowPopupUI<UIPurchasePopup>();
        //purchasePopup.Show(item);
    }
}
