using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMenu : MonoBehaviour
{
    [SerializeField] private Button btn_inventory;
    [SerializeField] private Button btn_Enhance;
    [SerializeField] private Button btn_Skill;

    private void Awake()
    {
        btn_inventory.onClick.AddListener(OnInventory);
        btn_Enhance.onClick.AddListener(OnEnhance);
        btn_Skill.onClick.AddListener(OnSkill);
    }


    public void OnInventory()
    {
        Debug.Log("Is Main Screan");
        UIManager.Instance.ShowPopupUI<UIPopupInventory>();
    }
    public void OnEnhance()
    {
        Debug.Log("강화창");
        UIManager.Instance.ShowPopupUI<UIEquipmentEnhance>();

    }

    public void OnSkill()
    {
        Debug.Log("스킬창");
        UIManager.Instance.ShowPopupUI<UISkillInventory>();

    }

}
