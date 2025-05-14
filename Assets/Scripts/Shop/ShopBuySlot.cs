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
        }

        iconImage.sprite = currentItemData.item.Icon;
        Txt_name.text = currentItemData.item.itemName;
        Txt_gold.text = currentItemData.item.gold.ToString();
        Txt_Tier.text = $"Tier: {currentItemData.item.Tier.ToString()} / 수량 : {currentItemData.amount}";
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
