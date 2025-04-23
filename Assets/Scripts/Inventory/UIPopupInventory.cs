using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIPopupInventory : PopupUI
{
    [SerializeField] private TextMeshProUGUI inventoryVolume;
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private TextMeshProUGUI gold;
    [SerializeField] private Image posionUI;
    [SerializeField] private Image slotEquipWeapon;
    [SerializeField] private Image slotEquipCoat;
    [SerializeField] private Image slotEquipShoes;
    [SerializeField] private Image slotEquipGlove;
    [SerializeField] private Image Slot_Equip_Relics_01;
    [SerializeField] private Image Slot_Equip_Relics_02;
    [SerializeField] private Image Slot_Equip_Relics_03;
    [SerializeField] private Button AddSlotBtn;

    // 탭 버튼들
    [SerializeField] private Button TotalTabButton;
    [SerializeField] private Button equipmentTabButton;
    [SerializeField] private Button consumableTabButton;
    [SerializeField] private Button RelicsTabButton;

    //각 class 요소들 
    private UIInventory uIInventory;
    private EquipMananger equipMananger;
    private InventoryManager inventoryMananger;
    private PlayerManager playerManager;
    
    private UIHUD uIHUD;
    public enum InventoryTabType
    {
        All,        // 전체
        Equipment,  // 장비
        Consumable, // 소비
        Relics    // 재료
    }

    protected void Awake()
    {
        Inittialize();
    }

    private void Inittialize()
    {
        uIInventory = GetComponentInChildren<UIInventory>();
        equipMananger = GameManager.Instance.EquipMananger;
        inventoryMananger = GameManager.Instance.InventoryManager;
        playerManager = GameManager.Instance.PlayerManager;
        
        uIHUD =FindObjectOfType<UIHUD>();
        uIInventory.UpdateInventory(inventoryMananger);


        // 탭 버튼 이벤트 등록
        if (TotalTabButton != null)
            TotalTabButton.onClick.AddListener(() => OnTabChanged(InventoryTabType.All));

        if (equipmentTabButton != null)
            equipmentTabButton.onClick.AddListener(() => OnTabChanged(InventoryTabType.Equipment));

        if (consumableTabButton != null)
            consumableTabButton.onClick.AddListener(() => OnTabChanged(InventoryTabType.Consumable));

        if (RelicsTabButton != null)
            RelicsTabButton.onClick.AddListener(() => OnTabChanged(InventoryTabType.Relics));

        if (AddSlotBtn != null)
        {
            AddSlotBtn.onClick.AddListener(() => OnAddSlot());
        }
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(() => OnCloseButtonClick());
        }

        // 이벤트 핸들러 등록
        GameManager.Instance.EquipMananger.OnEquipedChanged += HandleSingleItemChanged;
        GameManager.Instance.EquipMananger.OnRelicsChanged += HandleRelicsChanged; // 유물 장착/해제 이벤트 핸들러 등록
        inventoryMananger.OnSlotChanged += uIInventory.InitSlotShow;
        inventoryMananger.OnSlotChanged += HandleSlotChanged;
        inventoryMananger.OnSlotChanged += RefreshInventory;
        


    }

    protected override void OnEnable()
    {
        //초기화 작업
        base.OnEnable();



        // 인벤토리가 활성화될 때마다 최신 정보로 업데이트
        RefreshInventory();

        // 기본 탭 선택
        OnTabChanged(InventoryTabType.All);
    }

    protected override void Init()
    {
        base.Init();

    }

    public void OnItemSelected(ItemData item)
    {
        if (item == null) return;
        ItemData selecteditmed = null;
        
        if (item.itemType == ItemType.Equipment)
        {
            if (GameManager.Instance.EquipMananger.EquipDicionary.TryGetValue(item.equipType, out ItemData equipitem1))
            {
                selecteditmed= equipitem1;
            }
        }
        else if (item.itemType == ItemType.Relics)
        {
            //유물일때 
            if (GameManager.Instance.EquipMananger.HasFreeRelicSlot())
            {
                selecteditmed = item;
            }
        }


        // 장착한 아이템이 아닌것을 선택할때 
        // 장착된 아이템이 없다면?
        if (GameManager.Instance.EquipMananger.EquipDicionary.TryGetValue(item.equipType, out ItemData equipitem))
            {
                var selectedItemPopup = UIManager.Instance.ShowPopupUI<UISelectedItem>();
                selectedItemPopup.EquipBtn_interactable_true();
                //선택한 아이템이 장착아이템과 같은가 ?
                if (equipitem.id == item.id)
                {
                    selectedItemPopup.EquipBtn_interactable_flase();
                }
                if (selectedItemPopup != null)
                {
                    selectedItemPopup.Show(item, this);
                }
                var equipitempopup = UIManager.Instance.ShowPopupUI<UIEquipedItem>();
                if (equipitempopup != null)
                {
                    equipitempopup.Show(GameManager.Instance.EquipMananger.EquipDicionary[item.equipType]);
                }
            }
            else
            {
                var selectedItemPopup = UIManager.Instance.ShowPopupUI<UISelectedItem>();
                selectedItemPopup.EquipBtn_interactable_true();
                UIManager.Instance.ClosePopupUI<UIEquipedItem>();
                //내가 장착한 아이템이 없을때 
                if (selectedItemPopup != null)
                {
                    selectedItemPopup.Show(item, this);
                }
            }

    }

    // 탭 변경 처리
    public void OnTabChanged(InventoryTabType tabType)
    {
        // 선택된 탭에 따라 아이템 필터링 및 UI 업데이트
        Debug.Log($"탭 변경: {tabType}");

        // 버튼 시각적 상태 업데이트
        TotalTabButton.interactable = (tabType != InventoryTabType.All);
        equipmentTabButton.interactable = (tabType != InventoryTabType.Equipment);
        consumableTabButton.interactable = (tabType != InventoryTabType.Consumable);
        RelicsTabButton.interactable = (tabType != InventoryTabType.Relics);

        // 아이템 필터링 및 표시
        FilterItems(tabType);
    }

    // 아이템 필터링 메서드 추가
    private void FilterItems(InventoryTabType tabType)
    {
        if (uIInventory == null) return;

        // 인벤토리에 필터링 정보 전달
        uIInventory.FilterByTabType(tabType);
    }

    // 인벤토리 새로고침
    private void RefreshInventory()
    {
        // 플레이어 정보 업데이트
        playerName.text = playerManager.PlayerName;
        gold.text = playerManager.Currency.currencies[CurrencyType.Gold].ToString();
        inventoryVolume.text = $"{inventoryMananger.CountingSlotItemData()}/{inventoryMananger.ReturnTotalSlotCount()}";

        // 장비 슬롯 업데이트
        // UpdateEquipmentSlots();
        HandleAllEquipmentReset();
    }

    // 오버라이드: 닫기 버튼 클릭 처리
    protected override void OnCloseButtonClick()
    {
        // 특별한 처리가 필요한 경우 여기에 추가
        Debug.Log("인벤토리 닫힘");
        //메뉴창도 닫아주기
        uIHUD.OnMenu();
        UIManager.Instance.CloseAllPopupUI();
        // 부모 클래스의 메서드 호출
        //base.OnCloseButtonClick();
    }

    protected override void Clear()
    {
        base.Clear();
    }

    /// <summary>
    /// 특정 슬롯 1개만 업데이트 해주기  / Add =true / Remove =flase
    /// </summary>
    /// <param name="equipType"></param>
    /// <param name="itemData"></param>
    private void HandleSingleItemChanged(EquipType equipType, ItemData itemData, bool AddorRemove)
    {
        if (AddorRemove == true)
        {
            // 모든 장비 슬롯 초기화
            //slotEquipWeapon.sprite = null;
            //slotEquipCoat.sprite = null;
            //slotEquipShoes.sprite = null;
            //slotEquipGlove.sprite = null;

            //// 슬롯별 기본 색상 설정 - 비어있을 때는 약간 투명하게
            //Color slotEmptyColor = new Color(1f, 1f, 1f, 0.5f);
            //slotEquipWeapon.color = slotEmptyColor;
            //slotEquipCoat.color = slotEmptyColor;
            //slotEquipShoes.color = slotEmptyColor;
            //slotEquipGlove.color = slotEmptyColor;
            

            switch (equipType)
            {
                case EquipType.Weapon:
                    slotEquipWeapon.sprite = itemData.Icon;
                    //slotEquipWeapon.color = Color.white;
                    break;
                case EquipType.Coat:
                    slotEquipCoat.sprite = itemData.Icon;
                    //slotEquipWeapon.color = Color.white;
                    break;
                case EquipType.Shoes:
                    slotEquipShoes.sprite = itemData.Icon;
                    //slotEquipWeapon.color = Color.white;
                    break;
                case EquipType.Glove:
                    slotEquipGlove.sprite = itemData.Icon;
                    //slotEquipWeapon.color = Color.white;
                    break;
                default:
                    break;
            }
        }
        else
        {
            switch (equipType)
            {
                case EquipType.Weapon:
                    slotEquipWeapon.sprite = null;
                    break;
                case EquipType.Coat:
                    slotEquipCoat.sprite = null;
                    break;
                case EquipType.Shoes:
                    slotEquipShoes.sprite = null;
                    break;
                case EquipType.Glove:
                    slotEquipGlove.sprite = null;
                    break;
                default:
                    break;
            }
        }

        //OnTabChanged(InventoryTabType.All);
    }
    /// <summary>
    /// 전체 아이템 슬롯 업데이트 /처음에 아이템 초기화를 할때 해야겠지 ?
    /// </summary>
    private void HandleAllEquipmentReset()
    {
        if (equipMananger != null)
        {
            // 모든 장비 슬롯 초기화
            slotEquipWeapon.sprite = null;
            slotEquipCoat.sprite = null;
            slotEquipShoes.sprite = null;
            slotEquipGlove.sprite = null;

            // 유물 슬롯 초기화
            Slot_Equip_Relics_01.sprite = null;
            Slot_Equip_Relics_02.sprite = null;
            Slot_Equip_Relics_03.sprite = null;
            
            // 슬롯별 기본 색상 설정 - 비어있을 때는 약간 투명하게
            Color slotEmptyColor = new Color(1f, 1f, 1f, 1f);
            slotEquipWeapon.color = slotEmptyColor;
            slotEquipCoat.color = slotEmptyColor;
            slotEquipShoes.color = slotEmptyColor;
            slotEquipGlove.color = slotEmptyColor;
            Slot_Equip_Relics_01.color = slotEmptyColor;
            Slot_Equip_Relics_02.color = slotEmptyColor;
            Slot_Equip_Relics_03.color = slotEmptyColor;

            // 일반 장비 아이템 슬롯 업데이트
            foreach (var item in equipMananger.EquipDicionary.Values)
            {
                switch (item.equipType)
                {
                    case EquipType.None:
                        break;
                    case EquipType.Weapon:
                        slotEquipWeapon.sprite = item.Icon;
                        slotEquipWeapon.color = Color.white; 
                        break;
                    case EquipType.Coat:
                        slotEquipCoat.sprite = item.Icon;
                        slotEquipCoat.color = Color.white; 
                        break;
                    case EquipType.Shoes:
                        slotEquipShoes.sprite = item.Icon;
                        slotEquipShoes.color = Color.white;
                        break;
                    case EquipType.Glove:
                        slotEquipGlove.sprite = item.Icon;
                        slotEquipGlove.color = Color.white;
                        break;
                    default:
                        break;
                }
            }

            // 유물 아이템 슬롯 업데이트
            List<ItemData> equippedRelics = equipMananger.GetAllEquippedRelics();
            
            // 장착된 유물 개수에 따라 슬롯에 아이콘 표시
            for (int i = 0; i < equippedRelics.Count; i++)
            {
                switch (i)
                {
                    case 0:
                        Slot_Equip_Relics_01.sprite = equippedRelics[i].Icon;
                        Slot_Equip_Relics_01.color = Color.white; 
                        break;
                    case 1:
                        Slot_Equip_Relics_02.sprite = equippedRelics[i].Icon;
                        Slot_Equip_Relics_02.color = Color.white; 
                        break;
                    case 2:
                        Slot_Equip_Relics_03.sprite = equippedRelics[i].Icon;
                        Slot_Equip_Relics_03.color = Color.white;
                        break;
                }
            }
        }
    }

    // OnRelicsChanged 이벤트 핸들러 추가 (유물 장착/해제 시 호출됨)
    private void HandleRelicsChanged(ItemData relicItem, bool isEquipped)
    {
        // 유물 슬롯 UI 전체 업데이트
        HandleAllEquipmentReset();
    }

    private void OnAddSlot()
    {
        UIManager.Instance.ShowPopupUI<UIItemSlotAdd>();
    }

    private void HandleSlotChanged()
    {
        OnTabChanged(InventoryTabType.All);
    }


}