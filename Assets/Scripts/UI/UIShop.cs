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


    [SerializeField] private Transform SlotParent;
    [SerializeField] private UISlot slotPrefab;
    private List<UISlot> slots;
    private Shop Shop;
    private Player player;



    public void Initialize(Shop shop)
    {
        Shop = shop;
        ShowShopItems();
    }
    public void ShowShopItems()
    {
        RemoveSlots();
        slots= new List<UISlot>();
        var availableItems = Shop.GetAvailableItems();

        foreach (var item in availableItems)
        {
            var slot = Instantiate(slotPrefab, SlotParent);
            slot.SetSlotData(item);
            slot.OnItemClicked += OnShopItemSelected;
            slots.Add(slot);
        }

        //Grid 설정 
        SetupGridLayout();
    }

    private void SetupGridLayout()
    {
        GridLayoutGroup grid = SlotParent.GetComponent<GridLayoutGroup>();
        if (grid != null)
        {
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 4;  // 한 줄에 4개의 슬롯
            grid.spacing = new Vector2(20, 20);

            // Content 크기 자동 조절
            //int totalSlots = Shop();
            //int rows = Mathf.CeilToInt(totalSlots / 4f);
            //float totalHeight = (grid.cellSize.y + grid.spacing.y) * rows;

            //RectTransform contentRect = SlotParent.GetComponent<RectTransform>();
            //contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, totalHeight);
        }
    }

    private void RemoveSlots()
    {
        {
            // 모든 슬롯의 이벤트 구독 해제
            foreach (var slot in slots)
            {
                if (slot != null)
                {
                    slot.OnItemClicked -= OnShopItemSelected;
                }
            }

            // 모든 슬롯 오브젝트 파괴
            foreach (var slot in slots)
            {
                if (slot != null)
                {
                    Destroy(slot.gameObject);
                }
            }
        }
    }

    private void OnShopItemSelected(SlotItemData item)
    {
        //아이템 정보를 보여주는 창 1개 띄운다. > 구매 창 

        var purchasePopup = UIManager.Instance.ShowPopupUI<UIPurchasePopup>();
        purchasePopup.Initialize(Shop);
        purchasePopup.Show(item);

    }
}
