using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMap : PopupUI
{
    [SerializeField] private MapManager mapManager;
    [SerializeField] private GameObject map;
    private void Start()
    {
        closeButton.onClick.AddListener(Closebtn);
    }

    private void OnEnable()
    {
        if (mapManager == null)
        {
            mapManager = GameManager.Instance.MapManager;
            mapManager.initStart(map);
        }
    }


    private void Closebtn()
    {
        base.Close();
    }
}
