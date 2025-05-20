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
    [SerializeField]private Image backGroundKillImage;
    public Image UICoolDown; // 쿨타임 표시할 이미지
    //[SerializeField] private Image UICoolDown; // 스킬 아이콘
    public Skill skill;
    public FloatingSkillJoystick joystick;
    private Player player;

    // 기본 빈 스킬 아이콘 (옵션)
    [SerializeField] private Sprite defaultEmptyIcon;

    private void Awake()
    {

    }
    private void Start()
    {
        player = FindObjectOfType<Player>();
        UICoolDown.sprite = defaultEmptyIcon;
        backGroundKillImage.sprite = defaultEmptyIcon;
    }

    private void Update()
    {
        UICoolDown.fillAmount = GetPercentage();
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
        float cooldownReduction = Mathf.Clamp01(player._playerStat.GetStatValue(PlayerStatType.SkillCooltime) / 100f);
        float realCooldown = maximumCooldown * (1f - cooldownReduction);

        return realCooldown > 0f ? currentCooldown / realCooldown : 0f;
    }

    
    public void ResetCondition()
    {
        if (skill == null)
        {
            Debug.Log($"{index + 1}번째 스킬 칸에 스킬이 할당되지 않았습니다.");
            return;
        }
        if (skill != null)
        {
            maximumCooldown = skill.maxCooldown * player._playerStat.GetStatValue(PlayerStatType.SkillCooltime);
            currentCooldown = skill.cooldown;
        }
        else
        {
            Debug.Log("Skill 이 null입니다.");
        }

        if (skill.icon != null)
        {
            //skillIcon.sprite = skill.icon;
            UICoolDown.sprite = skill.icon;
            backGroundKillImage.sprite = skill.icon;
            // 아이콘 완전 불투명으로 설정
            Color iconColor = UICoolDown.color;
            iconColor.a = 1f;
            UICoolDown.color = iconColor;
        }
        else
        {
            UICoolDown.sprite = defaultEmptyIcon;
            backGroundKillImage.sprite= defaultEmptyIcon;
            Debug.Log("스킬 아이콘이 없습니다.");
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
            if (defaultEmptyIcon != null && UICoolDown != null)
            {
                UICoolDown.sprite = defaultEmptyIcon;
                backGroundKillImage.sprite = defaultEmptyIcon;
            }

            // 쿨다운 바를 비움
            if (UICoolDown != null)
            {
                UICoolDown.fillAmount = 0;
            }

            return;
        }

        // 스킬이 있는 경우 정상적으로 표시
        if (UICoolDown != null && skill.icon != null)
        {
            UICoolDown.sprite = skill.icon;
        }

        // 쿨다운 바 업데이트
        if (UICoolDown != null)
        {
            UICoolDown.fillAmount = GetPercentage();
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
        if (UICoolDown != null)
        {
            // 기본 아이콘 또는 빈 아이콘으로 설정
            // skillConditions[index].skillIcon.sprite = defaultEmptyIcon;

            // 또는 투명하게 만들기
            Color iconColor = UICoolDown.color;
            iconColor.a = 0.3f; // 반투명으로 설정
            UICoolDown.color = iconColor;
        }
    }
}
