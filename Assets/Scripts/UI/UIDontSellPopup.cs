using TMPro;
using UnityEngine;

public class UIDontSellPopup : PopupUI
{
    [SerializeField] private TextMeshProUGUI txt_DontSell;
    private void Awake()
    {
        closeButton.onClick.AddListener(OnCloseButtonClick);
    }

    protected override void OnCloseButtonClick()
    {
        base.OnCloseButtonClick();
    }
}
