using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISkillInformation : PopupUI
{
    [SerializeField] private Image Icon;
    [SerializeField] private Button Btn_EquipSkill;
    [SerializeField] private Button Btn_UnequipSkill;
    [SerializeField] private TextMeshProUGUI Name;
    [SerializeField] private TextMeshProUGUI Txt_Duscrition;
    [SerializeField] private TMP_Dropdown SlotDropdown;


    [SerializeField] private int selectedSlotIndex = 0;

    SkillInstance CurrentSkill;
    SkillManager skillManager;

    private void Start()
    {
        skillManager = GameManager.Instance.SkillManager;
        closeButton.onClick.AddListener(closebtn);
        Btn_EquipSkill.onClick.AddListener(OnEquipSkill);
        Btn_UnequipSkill.onClick.AddListener(OnunEquipSkill);

        if (SlotDropdown != null)
        {
            InitSlotDropdown();
        }
    }

    private void InitSlotDropdown()
    {
        if (SlotDropdown == null) return;

        SlotDropdown.ClearOptions();

        List<string> options = new List<string>();
        int slotCount = skillManager.ReturnTotalSlotCount();

        for (int i = 0; i < slotCount; i++)
        {
            options.Add($"슬롯 {i + 1}");
        }

        SlotDropdown.AddOptions(options);
        SlotDropdown.onValueChanged.AddListener(OnSlotSelected);
    }

    private void OnSlotSelected(int index)
    {
        selectedSlotIndex = index;
    }

    public void Init(SkillInstance _skillInstance)
    {
        CurrentSkill = _skillInstance;
        base.Show();
        UpdateUI();
        UpdateDescrtion();
    }

    private void OnEquipSkill()
    {
        if (CurrentSkill == null || skillManager == null)
            return;

        int slotToUse = SlotDropdown != null ? SlotDropdown.value : selectedSlotIndex;

        bool success = skillManager.EquipSkill(CurrentSkill.index, slotToUse);

        if (success)
        {
            //Debug.Log($"{CurrentSkill.skill.name} 스킬을 슬롯 {slotToUse + 1}에 장착했습니다.");
            base.OnCloseButtonClick();
        }
        else
        {
            Debug.Log("스킬 장착에 실패했습니다.");
        }
    }

    private void OnunEquipSkill()
    {
        if (skillManager == null)
            return;

        int slotToUse = SlotDropdown != null ? SlotDropdown.value : selectedSlotIndex;

        Skill slotSkill = skillManager.GetSkillAtSlot(slotToUse);

        if (slotSkill != null && slotSkill == CurrentSkill.skill)
        {
            skillManager.UnequipSkill(slotToUse);
            Debug.Log($"슬롯 {slotToUse + 1}에서 스킬을 해제했습니다.");
            base.OnCloseButtonClick();
        }
        else
        {
            Debug.Log("선택한 슬롯에 현재 스킬이 장착되어 있지 않습니다.");
        }
    }

    private void UpdateUI()
    {
        if (CurrentSkill == null) return;

        Icon.sprite = CurrentSkill.skill.icon;
        Name.text = CurrentSkill.skill.name;

        UpdateEquipButtons();
    }

    private void UpdateEquipButtons()
    {
        if (skillManager == null || CurrentSkill == null) return;

        bool isEquipped = false;

        for (int i = 0; i < skillManager.ReturnTotalSlotCount(); i++)
        {
            Skill slotSkill = skillManager.GetSkillAtSlot(i);
            if (slotSkill != null && slotSkill == CurrentSkill.skill)
            {
                isEquipped = true;
                selectedSlotIndex = i;

                if (SlotDropdown != null)
                {
                    SlotDropdown.value = i;
                }
                break;
            }
        }

        if (Btn_EquipSkill != null) Btn_EquipSkill.interactable = !isEquipped;
        if (Btn_UnequipSkill != null) Btn_UnequipSkill.interactable = isEquipped;
    }

    private void UpdateDescrtion()
    {
        Txt_Duscrition.text =
            $"스킬 데미지 : 공격력 * ({CurrentSkill.skill.description}) \n" +
            $"마나 소모량 : {CurrentSkill.skill.requiredMana}\n" +
            $"스킬 쿨타임 : {CurrentSkill.skill.maxCooldown}";

    }

    private void closebtn()
    {
        base.OnCloseButtonClick();
    }
}

