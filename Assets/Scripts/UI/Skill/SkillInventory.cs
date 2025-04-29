using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillInventory : MonoBehaviour
{
    public List<UISkillSlot> slots;
    //public int MaxSlots = 12;
    [SerializeField] private UISkillSlot uiSlotPrefab;
    [SerializeField] private Transform SlotParent;

    private bool isInitialized = false;

    private SkillManager skillManager;

    private void Start()
    {
        skillManager = GameManager.Instance.SkillManager;
        InitSlots();
    }

    // 초기화 (한 번만 실행)
    public void InitSlots()
    {
        if (!isInitialized)
        {
            InitSlotShow();
            isInitialized = true;
        }
    }

    public void InitSlotShow()
    {
        RemoveSlots();
        slots = new List<UISkillSlot>();
        for (int i = 0; i < skillManager.ReturnTotalSlotCount(); i++)
        {
            UISkillSlot slotobj = Instantiate(uiSlotPrefab, SlotParent);
            slots.Add(slotobj);
            slotobj.OnItemClicked += HandleItemOneClick;
        }

        
        // Grid Layout 및 Content 크기 업데이트
        SetupGridLayout();
    }

    public void ShowSkillInventory()
    {
        ResetSlots();

        for (int i = 0; i < skillManager.ReturnTotalSlotCount(); i++)
        {
            slots[i].SetSlotData(skillManager.skillInstances[i]);
        }
    }

    private void ResetSlots()
    {
        // 모든 슬롯 비우기
        foreach (var slot in slots)
        {
            slot.ClearSlot();
        }
    }

    /// <summary>
    /// 아이템 클릭시 발생하는 이벤트
    /// </summary>
    /// <param name="_skill"></param>
    private void HandleItemOneClick(SkillInstance _skill)
    {
        //스킬 정보 Popup 띄워주기 
    }

    private void RemoveSlots()
    {
        {
            // 모든 슬롯의 이벤트 구독 해제
            foreach (var slot in slots)
            {
                if (slot != null)
                {
                    slot.OnItemClicked -= HandleItemOneClick;
                }
            }

            // 모든 슬롯 오브젝트 파괴
            foreach (var slot in slots)
            {
                if (slot != null)
                {
                    Destroy(slot.gameObject);
                }
            }
        }
    }
    private void SetupGridLayout()
    {
        GridLayoutGroup grid = SlotParent.GetComponent<GridLayoutGroup>();
        if (grid != null)
        {
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 4;  // 한 줄에 4개의 슬롯
            grid.spacing = new Vector2(20, 20);

            // Content 크기 자동 조절
            int totalSlots = skillManager.ReturnTotalSlotCount();
            int rows = Mathf.CeilToInt(totalSlots / 4f);
            float totalHeight = (grid.cellSize.y + grid.spacing.y) * rows;

            RectTransform contentRect = SlotParent.GetComponent<RectTransform>();
            contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, totalHeight);
        }
    }
}
