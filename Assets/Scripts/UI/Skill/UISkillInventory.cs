using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISkillInventory : PopupUI
{

    private FloatingJoystick[] allFloatingjoystick;
    private void Start()
    {
        closeButton.onClick.AddListener(CloseBtn);
        
        // 비활성화된 오브젝트 포함 모든 오브젝트 찾기
        allFloatingjoystick = Resources.FindObjectsOfTypeAll<FloatingJoystick>();
    }


    private void CloseBtn()
    {
        base.OnCloseButtonClick();
        //조이스틱 UI켜기
        foreach (var item in allFloatingjoystick)
        {
            item.gameObject.SetActive(true);
        }
    }
}
