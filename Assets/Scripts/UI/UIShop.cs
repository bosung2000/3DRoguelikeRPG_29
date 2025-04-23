using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIShop : PopupUI
{
    //판매 나의인벤토리 
    //구매 shop가지고 있는 아이템 
    //판매해야할 아이템 list 가지고있고 
    //buy > inventory읽어와서 show 
    //sell > uishop(판매list)이중에서 색출해서(뭐 등급이 맞게 높은 등급일수록 좋은아이템이 나와야하잔아)
    // 그럼 아아템의 등급이 있어야하겠는데 > tier
    [SerializeField] TextMeshProUGUI goldTxt;
    [SerializeField] private ShopBuyInventory shopBuyInventory; //구매창 
    
    private Shop shop;
    private PlayerManager playerManager;

    private void Awake()
    {
        shopBuyInventory =GetComponentInChildren<ShopBuyInventory>();
    }

    public void Initialize(Shop _shop,PlayerManager _playerManager)
    {
        shop = _shop;
        playerManager = _playerManager;        
        shop.ShopitemChange += ShowShopGold;
        ShowShopGold();

        if (shopBuyInventory !=null)
        {
            shopBuyInventory.Initialize(shop);
        }

        closeButton.onClick.AddListener(OnCloseButtonClick);

    }
    //UI 다시 보여주는 로직 만들어야됨 >변경되는곳에 이벤트를 달아주자 slotdata가 

    protected override void OnCloseButtonClick()
    {
        UIManager.Instance.CloseAllPopupUI();
        base.OnCloseButtonClick();
    }

    public void ShowShopGold()
    {
        goldTxt.text = playerManager.Currency.currencies[CurrencyType.Gold].ToString();
    }

}
