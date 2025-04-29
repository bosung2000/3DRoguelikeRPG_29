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

    SkillInstance CurrentSkill;
    SkillManager skillManager;
    private void Start()
    {
        skillManager=GameManager.Instance.SkillManager;
        closeButton.onClick.AddListener(closebtn);
        Btn_EquipSkill.onClick.AddListener(OnEquipSkill);
        Btn_UnequipSkill.onClick.AddListener(OnunEquipSkill);
    }

    public void Init(SkillInstance _skillInstance)
    {
        CurrentSkill = _skillInstance;
        base.Show();
        UpdateUI();
    }


    private void OnEquipSkill()
    {
        //skillManager.EquipSkill();
    }
    private void OnunEquipSkill()
    {

    }

    private void UpdateUI()
    {
        Icon.sprite = CurrentSkill.skill.icon;
        Name.text = CurrentSkill.skill.name;
    }

    private void closebtn()
    {
        base.OnCloseButtonClick();
    }

}
