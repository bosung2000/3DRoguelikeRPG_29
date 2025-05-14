using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISlot : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Button SlotButton;
    [SerializeField] private TextMeshProUGUI txt_equip;
    [SerializeField] private TextMeshProUGUI amount;

    //슬롯별로 데이터를 가지고 있어야한다.
    //핸드폰 게임이니까 클릭을 했을때 ui를 1개 더 만들어서 띄우자

    public SlotItemData currentItemData;

    //아이템 클릭 이벤트 
    public Action<SlotItemData> OnItemClicked;

    private void Start()
    {

        txt_equip.gameObject.SetActive(false);
        amount.gameObject.SetActive(false);
        Init();
    }


    public void Init()
    {
        SlotButton.onClick.AddListener(onSlotClick);

    }

    private void onSlotClick()
    {
        if (currentItemData == null || currentItemData.IsEmpty)
        {
            Debug.Log("빈 슬롯 클릭");
            return;
        }

        OnItemClicked?.Invoke(currentItemData);

    }

    public void SetSlotData(SlotItemData slotData)
    {
        currentItemData = slotData;

        // 존재한다면
        if (!slotData.IsEmpty)
        {
            // 장착 표시 초기화
            txt_equip.gameObject.SetActive(false);

            //장착아이템 일때 검사
            if (GameManager.Instance.EquipMananger.EquipDicionary.TryGetValue(currentItemData.item.equipType, out ItemData item))
            {
                //Debug.Log($"장착된 아이템 ID: {item.id}");
                if (item.id == currentItemData.item.id)
                {
                    txt_equip.gameObject.SetActive(true);
                    //LayoutRebuilder.ForceRebuildLayoutImmediate(txt_equip.transform.parent as RectTransform);
                    //Debug.Log($"txt_equip 초기 상태: {txt_equip.gameObject.activeSelf}");
                }
            }
            // 장착 유물일때
            else if (GameManager.Instance.EquipMananger.RelicsDictionary.TryGetValue(currentItemData.item.id, out ItemData relics))
            {
                txt_equip.gameObject.SetActive(true);
            }

            iconImage.sprite = slotData.item.Icon;
            iconImage.enabled = true;

        }
        else
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
            txt_equip.gameObject.SetActive(false);
            amount.gameObject.SetActive(false);
        }
    }

    internal void ClearSlot()
    {
        // 슬롯 데이터 초기화
        SetSlotData(new SlotItemData());

        // 또는 필요한 경우 직접 초기화
        //iconImage.sprite = null;
        //iconImage.enabled = false;
        //amount.text = "";
    }


}
