using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SkillCondition : MonoBehaviour
{
    public float currentCooldown; //현재 쿨타임
    public float maximumCooldown; //최대 쿨타임
    public int index;
    public Image uiBar; // 쿨타임 표시할 이미지
    [SerializeField] private Image skillIcon; // 스킬 아이콘
    public Skill skill;
    public FloatingSkillJoystick joystick;
    
    // 기본 빈 스킬 아이콘 (옵션)
    [SerializeField] private Sprite defaultEmptyIcon; 

    private void Awake()
    {
        //만약 스킬아이콘이 지정되어있지 않다면, 객체의 이미지 컴포넌트 가져오기
        //if (skillIcon == null)
        //{
        //    skillIcon = this.GetComponent<Image>();
        //}
        //만약 쿨다운 이미지가 지정되어있지 않다면 마찬가지로 가져오기
        //if (uiBar==null)
        //{
        //    uiBar = this.transform.Find("UICooldown").GetComponent<Image>();
        //}
    }
    private void Start()
    {
        //skillIcon.sprite = skill.icon;
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
        return maximumCooldown > 0 ? currentCooldown / maximumCooldown : 0;
    }

    public void ResetCondition()
    {
        if (skill == null)
        {
            Debug.Log($"{index + 1}번째 스킬 칸에 스킬이 할당되지 않았습니다.");
            return;
        }
        if (skill.icon != null)
        {
            skillIcon.sprite = skill.icon;
            
            // 아이콘 완전 불투명으로 설정
            Color iconColor = skillIcon.color;
            iconColor.a = 1f;
            skillIcon.color = iconColor;
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
    
    /// <summary>
    /// UI 상태를 갱신합니다.
    /// </summary>
    public void RefreshUI()
    {
        if (skill == null)
        {
            // 스킬이 없는 경우 기본 아이콘 표시
            if (defaultEmptyIcon != null && skillIcon != null)
            {
                skillIcon.sprite = defaultEmptyIcon;
            }
            
            // 쿨다운 바를 비움
            if (uiBar != null)
            {
                uiBar.fillAmount = 0;
            }
            
            return;
        }
        
        // 스킬이 있는 경우 정상적으로 표시
        if (skillIcon != null && skill.icon != null)
        {
            skillIcon.sprite = skill.icon;
        }
        
        // 쿨다운 바 업데이트
        if (uiBar != null)
        {
            uiBar.fillAmount = GetPercentage();
        }
    }

    public void ClearSkill()
    {
        // 스킬 참조 제거
        skill = null;

        // 쿨다운 초기화
        currentCooldown = 0;
        maximumCooldown = 0;

        // 아이콘 초기화 (기본 이미지로 설정)
        if (skillIcon != null)
        {
            // 기본 아이콘 또는 빈 아이콘으로 설정
            // skillConditions[index].skillIcon.sprite = defaultEmptyIcon;

            // 또는 투명하게 만들기
            Color iconColor = skillIcon.color;
            iconColor.a = 0.3f; // 반투명으로 설정
            skillIcon.color = iconColor;
        }
    }
}
