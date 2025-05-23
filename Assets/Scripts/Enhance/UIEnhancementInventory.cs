using System.Collections.Generic;
using UnityEngine;

public class UIEnhancementInventory : MonoBehaviour
{
    public List<UISlot> slots;
    //public int MaxSlots = 12;
    [SerializeField] private UISlot uiSlotPrefab;
    [SerializeField] private Transform SlotParent;

    private InventoryManager inventoryManager;
    private UIPopupInventory uiPopupInventory;
    private bool isInitialized = false;


    public UIEquipmentEnhance enhanceUI;

    private void Awake()
    {
        
        uiPopupInventory = gameObject.GetComponentInParent<UIPopupInventory>();
        inventoryManager = GameManager.Instance.InventoryManager;
    }


    private void Start()
    {
        inventoryManager.OnSlotChanged += UpdateInventory;
    }

    private void OnEnable()
    {
        
        UpdateInventory();
    }

    // 초기화 (한 번만 실행)
    public void InitSlots()
    {
        
        {
            InitSlotShow();
            isInitialized = true;
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
    private void InitSlotShow()
    {
        RemoveSlots();
        slots = new List<UISlot>();
        
        for (int i = 0; i < inventoryManager.ReturnTotalSlotCount(); i++)
        {
            UISlot slotobj = Instantiate(uiSlotPrefab, SlotParent);
            slots.Add(slotobj);
            slotobj.OnItemClicked += HandleItemOneClick;
        }
    }

    // 데이터 업데이트 (필요할 때마다 실행)
    public void UpdateInventory()
    {
        
        {
            InitSlots();
            FilterByTabType(UIPopupInventory.InventoryTabType.Equipment);
        }
    }
    /// <summary>
    /// 여기서는 데이터를 가지고 show 보여주기만 하는 곳 >> 데이터를 관리 >inventoryManager
    /// </summary>


    /// <summary>
    /// 아이템 클릭시 발생하는 이벤트
    /// </summary>
    /// <param name="slotItemData"></param>
    private void HandleItemOneClick(SlotItemData slotItemData)
    {
        
        //장착 무기 
        if (slotItemData.item.itemType == ItemType.Equipment)
        {
            enhanceUI.SetEquipment(slotItemData.item);
        } 
        else
        {
            Debug.Log("장비 타입이 아닙니다.");
        }

    }
    /// <summary>
    /// 인벤토리 탭 누르면 타입에 맞춰서 보여주기 (초기화는 ALL로 설정)
    /// </summary>
    /// <param name="tabType"></param>
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

                case UIPopupInventory.InventoryTabType.Relics:
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
}
    
