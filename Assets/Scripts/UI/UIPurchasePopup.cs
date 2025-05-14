using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPurchasePopup : PopupUI
{
    [SerializeField] private Image itemimage;
    [SerializeField] private TextMeshProUGUI txt_01;
    [SerializeField] private TextMeshProUGUI txt_02;
    [SerializeField] private TextMeshProUGUI txt_03;
    [SerializeField] private TextMeshProUGUI txt_04;
    [SerializeField] private TextMeshProUGUI txt_05;
    [SerializeField] private TextMeshProUGUI txt_06;
    [SerializeField] private TextMeshProUGUI txt_07;
    [SerializeField] private TextMeshProUGUI Price;
    [SerializeField] private Button btn_Buy;

    public event Action<ItemData> OnBuyEvent;

    SlotItemData currentSlotItem;
    Shop shop;
    ItemManager itemManager;

    private void Awake()
    {
        btn_Buy.onClick.AddListener(OnBuyItem);
        closeButton.onClick.AddListener(ClosePopup);
        
    }
    private void Start()
    {
        itemManager = GameManager.Instance.ItemManager;
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
                //이벤트 추가 
                OnBuyEvent?.Invoke(currentSlotItem.item);
                //itemlist에서 삭제 
                itemManager.RemoveItemList(currentSlotItem.item);
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
        InitStattext();

        if (currentSlotItem == null) return;

        // 아이콘 설정
        if (currentSlotItem.item.Icon != null)
        {
            itemimage.sprite = currentSlotItem.item.Icon;
        }

        // 가격 표시
        Price.text = $"가격: {currentSlotItem.item.gold}";

        // 옵션이 있는 아이템이면 모든 옵션을 순회하며 값이 있는 옵션만 표시
        if (currentSlotItem.item.options != null && currentSlotItem.item.options.Count > 0)
        {
            List<TextMeshProUGUI> statTexts = new List<TextMeshProUGUI> 
            { 
                txt_01, txt_02, txt_03, txt_04, txt_05, txt_06, txt_07 
            };
            
            // 모든 텍스트 필드 초기화
            foreach (var text in statTexts)
            {
                if (text != null)
                    text.gameObject.SetActive(false);
            }
            
            // 아이템 옵션 순회하며 값이 0보다 큰 옵션만 표시
            int displayIndex = 0;
            foreach (var option in currentSlotItem.item.options)
            {
                float value = option.GetValueWithLevel(currentSlotItem.item.enhancementLevel);
                
                // 값이 0이 아닌 옵션만 표시
                if (Mathf.Abs(value) > 0.001f && displayIndex < statTexts.Count)
                {
                    string optionName = GetOptionName(option.type);
                    statTexts[displayIndex].text = $"{optionName}: {value}";
                    statTexts[displayIndex].gameObject.SetActive(true);
                    displayIndex++;
                }
            }
        }
    }
    
    // 옵션 타입에 따른 이름 반환
    private string GetOptionName(ConditionType type)
    {
        switch (type)
        {
            case ConditionType.Power: return "공격력";
            case ConditionType.MaxMana: return "최대 마나";
            case ConditionType.Mana: return "마나";
            case ConditionType.MaxHealth: return "최대 체력";
            case ConditionType.Health: return "체력";
            case ConditionType.Speed: return "속도";
            case ConditionType.reduction: return "피해감소";
            case ConditionType.CriticalChance: return "치명타확률";
            case ConditionType.CriticalDamage: return "치명타피해";
            case ConditionType.absorp: return "흡혈량";
            case ConditionType.DMGIncrease: return "데미지 증가";
            case ConditionType.HPRecovery: return "HP 회복";
            case ConditionType.MPRecovery: return "MP 회복";
            case ConditionType.GoldAcquisition: return "골드 획득량";
            case ConditionType.SkillColltime: return "스킬 쿨타임";
            case ConditionType.AttackSpeed: return "공격 속도";
            default: return type.ToString();
        }
    }

    private void InitStattext()
    {
        //초기화 코드 
        itemimage.sprite = null;
        txt_01.text = null;
        txt_02.text = null;
        txt_03.text = null;
        txt_05.text = null;
        txt_06.text = null;
        txt_03.text = null;
        Price.text = null;
    }
}
