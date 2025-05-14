using UnityEngine;
using UnityEngine.UI;

public class UIAddNotSlot : PopupUI
{
    [SerializeField] private Button Btn_ok;
    private void Awake()
    {
        Btn_ok = GetComponentInChildren<Button>();
        Btn_ok.onClick.AddListener(() => OnClosePopup());
    }


    private void OnClosePopup()
    {
        UIManager.Instance.ClosePopupUI<UIAddNotSlot>();
    }




}
