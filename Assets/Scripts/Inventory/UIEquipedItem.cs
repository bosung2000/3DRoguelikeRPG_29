using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIEquipedItem : PopupUI
{
    [SerializeField] private Image itemimage;
    [SerializeField] private TextMeshProUGUI txt_01;
    [SerializeField] private TextMeshProUGUI txt_02;
    [SerializeField] private TextMeshProUGUI txt_03;
    [SerializeField] private TextMeshProUGUI txt_04;
    [SerializeField] private TextMeshProUGUI txt_05;
    [SerializeField] private TextMeshProUGUI txt_06;
    [SerializeField] private TextMeshProUGUI txt_07;

    private ItemData EquipItem;
    public void Show(ItemData item)
    {
        if (item == null) return;

        EquipItem = item;
        base.Show();
        UpdateUI();
    }

    public override void Hide()
    {
        EquipItem = null;
        base.Hide();
    }


    private void UpdateUI()
    {
        InitStattext();

        if (EquipItem == null) return;

        // 아이콘 설정
        if (EquipItem.Icon != null)
        {
            itemimage.sprite = EquipItem.Icon;
        }

        // 옵션이 있는 아이템이면 모든 옵션을 순회하며 값이 있는 옵션만 표시
        if (EquipItem.options != null && EquipItem.options.Count > 0)
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
            foreach (var option in EquipItem.options)
            {
                float value = option.GetValueWithLevel(EquipItem.enhancementLevel);

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

    protected override void Clear()
    {
        base.Clear();
        EquipItem = null;
    }
}
