using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SkillCondition : MonoBehaviour
{
    public float currentCooldown; //현재 쿨타임
    public float maximumCooldown; //최대 쿨타임
    public float startValue; //시작값
    public float passiveValue;
    public float plusValue;
    public int index;
    public Image uiBar; // 쿨타임 표시할 이미지
    public Image skillIcon; // 스킬 아이콘
    public Skill skill;

    private void Awake()
    {
        //만약 스킬아이콘이 지정되어있지 않다면, 객체의 이미지 컴포넌트 가져오기
        if (skillIcon == null)
        {
            skillIcon = this.GetComponent<Image>();
        }
        //만약 쿨다운 이미지가 지정되어있지 않다면 마찬가지로 가져오기
        if (uiBar==null)
        {
            uiBar = this.transform.Find("UICooldown").GetComponent<Image>();
        }
    }
    private void Start()
    {

    }

    private void Update()
    {
        uiBar.fillAmount = GetPercentage();
    }

    public void Add(float amount)
    {
        currentCooldown = Mathf.Min(currentCooldown + amount, maximumCooldown);
    }

    public void Subtract(float amount)
    {
        currentCooldown = Mathf.Max(currentCooldown - amount, 0.0f);
    }

    public float GetPercentage()
    {
        return currentCooldown / maximumCooldown;
    }

    public void ResetCondition()
    {
        if (skill==null)
        {
            Debug.Log("스킬이 없습니다.");
            return;
        }
        if (skill.icon != null)
        {
            skillIcon.sprite = skill.icon;
        }
        else
        {
            Debug.Log("스킬 아이콘이 없습니다.");
        }
        if (skill.cooldown != null)
        {
            maximumCooldown = skill.cooldown;
        }
        else
        {
            Debug.Log("스킬 쿨타임이 없습니다.");
        }
        if (skill.cooldown != null)
        {
            currentCooldown = skill.cooldown;
        }
        else
        {
            Debug.Log("스킬 쿨타임이 없습니다.");
        }
    }

}
