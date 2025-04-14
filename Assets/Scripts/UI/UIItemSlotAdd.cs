using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItemSlotAdd : PopupUI
{
    [SerializeField] private Button btn_Yes;
    [SerializeField] private Button btn_No;
    InventoryManager inventoryManager;
    private void Awake()
    {
        inventoryManager = GameManager.Instance.InventoryManager;
        if (btn_Yes != null)
        {
            btn_Yes.onClick.AddListener(() => OnAddSlot());
        }
        if (btn_No !=null)
        {
            btn_No.onClick.AddListener(() => OnClosePopup());
        }
    }


    private void OnEnable()
    {
        UIManager.Instance.RegisterUI(this);
    }

    public void Init()
    {

    }

    private void OnAddSlot()
    {
        inventoryManager.AddSlots();
        UIManager.Instance.ClosePopupUI<UIItemSlotAdd>();
    }

    private void OnClosePopup()
    {
        UIManager.Instance.ClosePopupUI<UIItemSlotAdd>();
    }
}
