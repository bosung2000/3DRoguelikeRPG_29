using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISkillSlot : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Button SlotButton;
    [SerializeField] private TextMeshProUGUI txt_equip;
    [SerializeField] private TextMeshProUGUI Level;

    public SkillInstance currentSkill;

    public Action<SkillInstance> OnItemClicked;

    void Start()
    {
        txt_equip.gameObject.SetActive(false);
        Level.gameObject.SetActive(false);
        Init();
    }

    public void Init()
    {
        SlotButton.onClick.AddListener(onSlotClick);
    }

    private void onSlotClick()
    {
        if (currentSkill == null)
        {
            Debug.Log("빈 슬롯 클릭");
            return;
        }

        OnItemClicked?.Invoke(currentSkill);

    }

    public void SetSlotData(SkillInstance _skill)
    {
        currentSkill = _skill;

        if (_skill.skill != null)
        {
            iconImage.sprite = _skill.skill.icon;
            iconImage.enabled = true;
        }
        else
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
            txt_equip.gameObject.SetActive(false);
            Level.gameObject.SetActive(false);
        }
    }

    internal void ClearSlot()
    {
        // 슬롯 데이터 초기화
        SetSlotData(new SkillInstance());
    }
}
