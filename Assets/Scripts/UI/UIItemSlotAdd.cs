using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIItemSlotAdd : PopupUI
{
    [SerializeField] private Button btn_Yes;
    [SerializeField] private Button btn_No;
    [SerializeField] private TextMeshProUGUI Txt_playerGold;
    [SerializeField] private TextMeshProUGUI Txt_AddCost;
    InventoryManager inventoryManager;
    PlayerManager playerManager;
    private void Awake()
    {
        inventoryManager = GameManager.Instance.InventoryManager;
        playerManager = GameManager.Instance.PlayerManager;
        if (btn_Yes != null)
        {
            btn_Yes.onClick.AddListener(() => OnAddSlot());
        }
        if (btn_No != null)
        {
            btn_No.onClick.AddListener(() => OnClosePopup());
        }
    }


    private void OnEnable()
    {
        UIManager.Instance.RegisterUI(this);
        Txt_playerGold.text = $"보유골드 : {playerManager.Currency.currencies[CurrencyType.Gold]}";
        Txt_AddCost.text = $"필요골드 : {inventoryManager.AddSlotCost}";
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
