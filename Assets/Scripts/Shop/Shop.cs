using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    private List<SlotItemData> availableItems = new List<SlotItemData>();
    [SerializeField] private int shopTier = 10;
    private PlayerManager playerManager;

    private void Start()
    {
        InitializeShop();
    }
    public void InitializeShop()
    {
        playerManager = GameManager.Instance.PlayerManager;
        availableItems.Clear();
        // List로 받아오고 이걸 slotitemdata형태로 변경해야됨 
        List<ItemData> items = ItemManager.Instance.GetItemsByTierRange(shopTier, 10);

        foreach (var item in items)
        {
            SlotItemData slotItemData = new SlotItemData(item, 1);
            availableItems.Add(slotItemData);
        }
    }

    public List<SlotItemData> GetAvailableItems()
    {
        return availableItems;
    }
    /// <summary>
    /// shoplist에서 삭제하기 
    /// </summary>
    public void RemoveShopItemlist(SlotItemData Slotitem)
    {
        if (availableItems.Contains(Slotitem))
        {
            availableItems.Remove(Slotitem);
        }
        else
        {
            Debug.Log("상점에 그러한 아이템이 존재하지 않습니다.");
        }
    }

    /// <summary>
    /// 골드 확인후 골드 차감까지 
    /// </summary>
    /// <param name="item"></param>
    /// <param name="player"></param>
    /// <returns></returns>
    public bool TryPurchaseItem(ItemData item)
    {
        if (playerManager.Currency.CanAfford(CurrencyType.Gold, item.gold))
        {
            playerManager.Currency.AddCurrency(CurrencyType.Gold, item.gold);
            return true;
        }
        return false;
    }


}
