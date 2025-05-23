using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UIPopupInventory;
using Debug = UnityEngine.Debug;

public class UISelectedItem : PopupUI
{
    [SerializeField] private Image itemimage;
    [SerializeField] private TextMeshProUGUI txt_01;
    [SerializeField] private TextMeshProUGUI txt_02;
    [SerializeField] private TextMeshProUGUI txt_03;
    [SerializeField] private TextMeshProUGUI txt_04;
    [SerializeField] private TextMeshProUGUI txt_05;
    [SerializeField] private TextMeshProUGUI txt_06;
    [SerializeField] private TextMeshProUGUI txt_07;
    [SerializeField] private Button btn_equip;
    [SerializeField] private Button btn_Release;
    private UIPopupInventory uiPopupInventory;

    private ItemData currentItem;

    protected void Awake()
    {
        btn_equip.onClick.AddListener(OnEquipButtonClicked);
        btn_Release.onClick.AddListener(OnReleaseButtonClicked);
        closeButton.onClick.AddListener(OncloseBtn);
        Initialize();
    }

    private void OncloseBtn()
    {
        UIManager.Instance.ClosePopupUI<UIEquipedItem>();
        base.OnCloseButtonClick();
    }

    public void Initialize()
    {
        // 초기화 로직
        
    }

    private void OnEnable()
    {
        btn_equip.interactable= true;
    }

    public void Show(ItemData item, UIPopupInventory _uIPopupInventory)
    {
        if (item == null) return;

        uiPopupInventory = _uIPopupInventory;
        currentItem = item;
        base.Show();
        UpdateUI();
    }

    public override void Hide()
    {
        currentItem = null;
        base.Hide();
    }

    private void UpdateUI()
    {
        InitStattext();

        if (currentItem == null) return;

        // 아이콘 설정
        if (currentItem.Icon != null)
        {
            itemimage.sprite = currentItem.Icon;
        }

        
        // 옵션이 있는 아이템이면 모든 옵션을 순회하며 값이 있는 옵션만 표시
        if (currentItem.options != null && currentItem.options.Count > 0)
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
            foreach (var option in currentItem.options)
            {
                float value = option.GetValueWithLevel(currentItem.enhancementLevel);

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
    }

    private void OnEquipButtonClicked()
    {
        if (currentItem != null)
        {
            //장착된 아이템은 다시장착하지 못하게 해야한다.
            //1.equipitem의 데이터를 안넣기 
            //2.장착 버튼을 비활성화 해주는것 

            //false 반환은 같은 아이템을 선택했다는것  >데이터를 안넣기 
            if (GameManager.Instance.EquipMananger.Equipitem(currentItem))
            {
                //UI초기화
                uiPopupInventory.OnTabChanged(InventoryTabType.All);
                // 팝업 닫기
                Debug.Log($"{currentItem.itemName} 장착");
                UIManager.Instance.ClosePopupUI(this);
                UIManager.Instance.ClosePopupUI<UIEquipedItem>();
            }
            else
            {
                Debug.Log("장착list에 아이템이 존재하지 않습니다");
            }
        }
        else
        {
            Debug.Log("선택된 아이템의 정보가 없습니다.");
        }
    }

    private void OnReleaseButtonClicked()
    {
        if (currentItem != null)
        {
            // 장착 list에서 제거 
            if (GameManager.Instance.EquipMananger.UnEquipitem(currentItem))
            {
                //UI초기화
                uiPopupInventory.OnTabChanged(InventoryTabType.All);
                Debug.Log($"{currentItem.itemName} 해제");
                // 팝업 닫기
                UIManager.Instance.ClosePopupUI(this);
                UIManager.Instance.ClosePopupUI<UIEquipedItem>();
            }
            else
            {
                Debug.Log("장착 list에 아이템이 존재하지 않습니다.");
            }
        }
    }

    protected override void Clear()
    {
        base.Clear();
        currentItem = null;
    }

    public void EquipBtn_interactable_flase()
    {
        btn_equip.interactable = false;
    }
    public void EquipBtn_interactable_true()
    {
        btn_equip.interactable = true;
    }
}
