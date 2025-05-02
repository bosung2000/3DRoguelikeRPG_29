using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISkillInventory : PopupUI
{
    private UIHUD uIHUD;
    private FloatingJoystick[] allFloatingjoystick;
    private void Start()
    {
        Init();
    }

    public void Init()
    {
        closeButton.onClick.AddListener(CloseBtn);

        // 비활성화된 오브젝트 포함 모든 오브젝트 찾기
        allFloatingjoystick = Resources.FindObjectsOfTypeAll<FloatingJoystick>();
        uIHUD = FindObjectOfType<UIHUD>();
    }


    private void CloseBtn()
    {
        //메뉴창도 닫아주기
        uIHUD.OnMenu();
        base.OnCloseButtonClick();
        UIManager.Instance.CloseAllPopupUI();
        //조이스틱 UI켜기
        foreach (var item in allFloatingjoystick)
        {
            item.gameObject.SetActive(true);
        }
    }
}
