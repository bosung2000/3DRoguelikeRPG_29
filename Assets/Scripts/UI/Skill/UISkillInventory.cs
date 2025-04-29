using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISkillInventory : PopupUI
{

    private void Start()
    {
        closeButton.onClick.AddListener(CloseBtn);
    }


    private void CloseBtn()
    {
        base.OnCloseButtonClick();
    }
}
