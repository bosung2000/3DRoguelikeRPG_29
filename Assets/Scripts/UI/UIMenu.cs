using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMenu : MonoBehaviour
{
    [SerializeField] private Button btn_inventory;
    [SerializeField] private Button btn_Enhance;
    [SerializeField] private Button btn_Skill;
    [SerializeField] private Button btn_Menu;

    private FloatingJoystick floatingJoystick;
    private UIHUD uiHUD;
    private void Awake()
    {
        uiHUD = GetComponentInParent<UIHUD>();
        btn_inventory.onClick.AddListener(OnInventory);
        btn_Enhance.onClick.AddListener(OnEnhance);
        btn_Skill.onClick.AddListener(OnSkill);
        btn_Menu.onClick.AddListener(OnMenu);
    }
    private void Start()
    {
        floatingJoystick = FindObjectOfType<FloatingJoystick>();
    }


    public void OnInventory()
    {
        
        UIManager.Instance.ShowPopupUI<UIPopupInventory>();
        //조이스틱 UI끄기
        floatingJoystick.gameObject.SetActive(false);
    }
    public void OnEnhance()
    {
        
        UIManager.Instance.ShowPopupUI<UIEquipmentEnhance>();
        //조이스틱 UI끄기
        floatingJoystick.gameObject.SetActive(false);
    }

    public void OnSkill()
    {
        
        UIManager.Instance.ShowPopupUI<UISkillInventory>();
        //조이스틱 UI끄기
        floatingJoystick.gameObject.SetActive(false);
    }
    public void OnMenu()
    {
        uiHUD.OnMenu();
        
    }

}
