using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UIPopupInventory;

public class ShopSellInventory : MonoBehaviour
{
    public List<UISlot> slots;
    [SerializeField] private UISlot uiSlotPrefab;
    [SerializeField] private Transform SlotParent;

    // 탭 버튼들
    [SerializeField] private Button TotalTabButton;
    [SerializeField] private Button equipmentTabButton;
    [SerializeField] private Button consumableTabButton;
    [SerializeField] private Button materialTabButton;


    private InventoryManager inventoryManager;
    private UIShop uIShop;

    private bool isInitialized = false;

    private void Awake()
    {
        uIShop = GetComponentInParent<UIShop>();
        Initialize();
        inventoryManager = GameManager.Instance.InventoryManager;
        inventoryManager.OnSlotChanged += InitSlotShow;
        inventoryManager.OnSlotChanged += HandleSlotChanged;


        InitSlots();
    }

    private void Initialize()
    {
        // 탭 버튼 이벤트 등록
        if (TotalTabButton != null)
            TotalTabButton.onClick.AddListener(() => OnTabChanged(InventoryTabType.All));

        if (equipmentTabButton != null)
            equipmentTabButton.onClick.AddListener(() => OnTabChanged(InventoryTabType.Equipment));

        if (consumableTabButton != null)
            consumableTabButton.onClick.AddListener(() => OnTabChanged(InventoryTabType.Consumable));

        if (materialTabButton != null)
            materialTabButton.onClick.AddListener(() => OnTabChanged(InventoryTabType.Material));
    }

    private void OnEnable()
    {
        OnTabChanged(InventoryTabType.All);
    }

    // 초기화 (한 번만 실행)
    public void InitSlots()
    {
        if (!isInitialized)
        {
            InitSlotShow();
            isInitialized = true;
        }

    }
    //인벤토리 아이템이 더해지커나 삭제됬을때 show이벤트를 등록해줘야한다.
    public void InitSlotShow()
    {
        Debug.Log("InitSlotShow 시작");
        RemoveSlots();
        slots = new List<UISlot>();
        for (int i = 0; i < inventoryManager.ReturnTotalSlotCount(); i++)
        {
            UISlot slotobj = Instantiate(uiSlotPrefab, SlotParent);
            // 슬롯 생성 후 바로 데이터 설정
            //if (i < inventoryManager.slotItemDatas.Count)
            //{
            //    slotobj.SetSlotData(inventoryManager.slotItemDatas[i]);
            //}
            slots.Add(slotobj);
            slotobj.OnItemClicked += HandleItemOneClick;

        }


        // Grid Layout 및 Content 크기 업데이트
        SetupGridLayout();
        Debug.Log("InitSlotShow 완료");
    }

    private void RemoveSlots()
    {
        {
            // 모든 슬롯의 이벤트 구독 해제
            foreach (var slot in slots)
            {
                if (slot != null)
                {
                    slot.OnItemClicked -= HandleItemOneClick;
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

    /// <summary>
    /// 아이템 클릭시 발생하는 이벤트
    /// </summary>
    /// <param name="slotItemData"></param>
    private void HandleItemOneClick(SlotItemData slotItemData)
    {
        //판매창 1개만 띄워주면됨 
        var uisellpopup = UIManager.Instance.ShowPopupUI<UISellPopup>();
        uisellpopup.Initialize(slotItemData, inventoryManager, this, uIShop);
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
            int totalSlots = inventoryManager.ReturnTotalSlotCount();
            int rows = Mathf.CeilToInt(totalSlots / 4f);
            float totalHeight = (grid.cellSize.y + grid.spacing.y) * rows;

            RectTransform contentRect = SlotParent.GetComponent<RectTransform>();
            contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, totalHeight);
        }
    }


    // 탭 변경 처리
    private void OnTabChanged(InventoryTabType tabType)
    {
        // 선택된 탭에 따라 아이템 필터링 및 UI 업데이트
        Debug.Log($"탭 변경: {tabType}");

        // 버튼 시각적 상태 업데이트
        TotalTabButton.interactable = (tabType != InventoryTabType.All);
        equipmentTabButton.interactable = (tabType != InventoryTabType.Equipment);
        consumableTabButton.interactable = (tabType != InventoryTabType.Consumable);
        materialTabButton.interactable = (tabType != InventoryTabType.Material);

        // 아이템 필터링 및 표시
        FilterByTabType(tabType);
    }

    public void FilterByTabType(UIPopupInventory.InventoryTabType tabType)
    {
        if (inventoryManager == null) return;

        // 모든 슬롯 초기화
        ResetSlots();

        // 필터링된 아이템 목록 생성
        List<SlotItemData> filteredItems = new List<SlotItemData>();

        foreach (var slotData in inventoryManager.slotItemDatas)
        {
            if (slotData.IsEmpty) continue;

            bool shouldInclude = false;

            switch (tabType)
            {
                case UIPopupInventory.InventoryTabType.All:
                    shouldInclude = true;  // 모든 아이템 포함
                    break;

                case UIPopupInventory.InventoryTabType.Equipment:
                    shouldInclude = (slotData.item.itemType == ItemType.Equipment);
                    break;

                case UIPopupInventory.InventoryTabType.Consumable:
                    shouldInclude = (slotData.item.itemType == ItemType.Consumable);
                    break;

                case UIPopupInventory.InventoryTabType.Material:
                    shouldInclude = (slotData.item.itemType == ItemType.Material);
                    break;
            }

            if (shouldInclude)
            {
                filteredItems.Add(slotData);
            }
        }

        // 필터링된 아이템을 슬롯에 표시
        DisplayFilteredItems(filteredItems);
    }



    private void DisplayFilteredItems(List<SlotItemData> filteredItems)
    {
        // 필터링된 아이템만 슬롯에 표시
        int slotIndex = 0;
        if (filteredItems.Count > slots.Count)
        {
            Debug.Log("슬롯이 부족합니다.");
        }
        for (int i = 0; i < filteredItems.Count && slotIndex < slots.Count; i++)
        {
            // 비어있지 않은 슬롯만 처리
            if (!filteredItems[i].IsEmpty)
            {
                slots[slotIndex].SetSlotData(filteredItems[i]);
                slotIndex++;
            }
        }
    }
    private void ResetSlots()
    {
        // 모든 슬롯 비우기
        foreach (var slot in slots)
        {
            slot.ClearSlot();
        }
    }
    private void HandleSlotChanged()
    {
        Debug.Log("HandleSlotChanged 시작");
        //FilterByTabType(InventoryTabType.All);
        OnTabChanged(InventoryTabType.All);
        Debug.Log("HandleSlotChanged 완료");
    }



}
