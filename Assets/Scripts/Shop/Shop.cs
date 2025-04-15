using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    private List<SlotItemData> availableItems = new List<SlotItemData>();
    private int shopTier = 10;

    public void InitializeShop()
    {
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
    /// 골드 확인후 골드 차감까지 
    /// </summary>
    /// <param name="item"></param>
    /// <param name="player"></param>
    /// <returns></returns>
    public bool TryPurchaseItem(ItemData item, PlayerManager player)
    {
        if (player.Currency.CanAfford(CurrencyType.Gold, item.gold))
        {
            player.Currency.AddCurrency(CurrencyType.Gold, item.gold);
            return true;
        }
        return false;
    }


}
