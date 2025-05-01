using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMenu : MonoBehaviour
{
    [SerializeField] private Button btn_inventory;
    [SerializeField] private Button btn_Enhance;
    [SerializeField] private Button btn_Skill;

    private FloatingJoystick floatingJoystick;
    private void Awake()
    {
        btn_inventory.onClick.AddListener(OnInventory);
        btn_Enhance.onClick.AddListener(OnEnhance);
        btn_Skill.onClick.AddListener(OnSkill);
    }
    private void Start()
    {
        floatingJoystick = FindObjectOfType<FloatingJoystick>();
    }


    public void OnInventory()
    {
        Debug.Log("Is Main Screan");
        UIManager.Instance.ShowPopupUI<UIPopupInventory>();
        //조이스틱 UI끄기
        floatingJoystick.gameObject.SetActive(false);
    }
    public void OnEnhance()
    {
        Debug.Log("강화창");
        UIManager.Instance.ShowPopupUI<UIEquipmentEnhance>();
        //조이스틱 UI끄기
        floatingJoystick.gameObject.SetActive(false);
    }

    public void OnSkill()
    {
        Debug.Log("스킬창");
        UIManager.Instance.ShowPopupUI<UISkillInventory>();
        //조이스틱 UI끄기
        floatingJoystick.gameObject.SetActive(false);
    }

}
