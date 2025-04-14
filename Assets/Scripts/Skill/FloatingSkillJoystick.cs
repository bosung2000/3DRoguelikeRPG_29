using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FloatingSkillJoystick : Joystick
{
    public int index;
    public SkillManager skillManager;

    public void Awake()
    {
        skillManager = FindAnyObjectByType<SkillManager>();

        if (skillManager != null )
        {
            Debug.Log("스킬 조작 UI에 스킬매니저가 제대로 들어갔습니다.");
        }
    }

    protected override void Start()
    {
        base.Start();
        background.gameObject.SetActive(false);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
        background.gameObject.SetActive(true);
        base.OnPointerDown(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        background.gameObject.SetActive(false);
        base.OnPointerUp(eventData);        
    }
}
