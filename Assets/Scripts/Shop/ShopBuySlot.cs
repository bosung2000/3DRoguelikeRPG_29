using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopBuySlot : MonoBehaviour
{
    [SerializeField] Button SlotButton;
    [SerializeField] Image iconImage;
    [SerializeField] TextMeshProUGUI Txt_name;
    [SerializeField] TextMeshProUGUI Txt_Tier;
    [SerializeField] TextMeshProUGUI Txt_gold;
    [SerializeField] Image Img_Gold;
    [SerializeField] Image Img_Soul;

    private ShopType shopType;
    //[SerializeField] TextMeshProUGUI Txt_amount;

    public SlotItemData currentItemData;

    //아이템 클릭 이벤트 
    public Action<SlotItemData> OnitemClicked;

    void Start()
    {

        Init();
    }

    public void Init()
    {
        SlotButton.onClick.AddListener(OnSlotClick);
    }
    public void SetCurrentype(ShopType _ShopType)
    {
        shopType = _ShopType;
    }

    private void OnSlotClick()
    {
        if (currentItemData == null ||currentItemData.IsEmpty)
        {
            Debug.Log("상점 빈 슬롯 클릭");
            return;
        }
        OnitemClicked?.Invoke(currentItemData);
    }

    public void SetSlotData_Equip(SlotItemData slotData)
    {
        currentItemData = slotData;
        //초기화
        if (slotData.IsEmpty)
        {
            iconImage.sprite = null;
            Txt_name = null;
            Txt_gold= null;
            Txt_Tier= null;
            Img_Gold.sprite = null;
            Img_Soul.sprite = null;
        }

        iconImage.sprite = currentItemData.item.Icon;
        Txt_name.text = currentItemData.item.itemName;
        Txt_gold.text = currentItemData.item.gold.ToString();
        Txt_Tier.text = $"Tier: {currentItemData.item.Tier.ToString()} / 수량 : {currentItemData.amount}";
        if (shopType ==ShopType.Eqyip)
        {
            Img_Gold.gameObject.SetActive(true);
            Img_Soul.gameObject.SetActive(false);
        }
        else if (shopType == ShopType.Relic)
        {
            Img_Gold.gameObject.SetActive(false);
            Img_Soul.gameObject.SetActive(true);
        }
    }
    public void SetSlotData_Relice(SlotItemData slotData)
    {
        currentItemData = slotData;
        //초기화
        if (slotData.IsEmpty)
        {
            iconImage.sprite = null;
            Txt_name = null;
            Txt_gold = null;
            Txt_Tier = null;
        }

        iconImage.sprite = currentItemData.item.Icon;
        Txt_name.text = currentItemData.item.name;
        Txt_gold.text = currentItemData.item.gold.ToString();
        Txt_Tier.text = $"Tier: {currentItemData.item.Tier.ToString()} / 수량 : {currentItemData.amount}";
    }
    public void ClearSlot()
    {
        SetSlotData_Equip(new SlotItemData());
        SetSlotData_Relice(new SlotItemData());
    }


}
