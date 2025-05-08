using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMap : PopupUI
{
    private void Start()
    {
        closeButton.onClick.AddListener(Closebtn);
    }


    private void Closebtn()
    {
        base.Close();
    }
}
