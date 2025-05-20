using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISkill : MonoBehaviour
{
    [SerializeField] private SkillCondition[] skillConditions;
    [SerializeField] private SkillManager skillManager;

    private void Awake()
    {
        //스킬 UI 설정
        skillConditions = new SkillCondition[this.transform.childCount];
        for (int i = 0; i < skillConditions.Length; i++)
        {
            skillConditions[i] = this.transform.GetChild(i).GetComponent<SkillCondition>(); //객체 안에서 SkillCondition 클래스를 찾아 지정하고
            skillConditions[i].index = i;
            skillConditions[i].joystick.index = i;
        }
    }
    /// <summary>
    /// 바뀐 enabledSkills에 맞게 스킬 UI 변경
    /// </summary>
    public void ResetSkillUI(int index, SkillInstance enabledSkills = null)
    {
        if (enabledSkills == null)
        {
            skillConditions[index].skill =null;
            skillConditions[index].ResetCondition();
        }
        else
        {
            skillConditions[index].skill = enabledSkills.skill;
            skillConditions[index].ResetCondition();
        }
    }

    /// <summary>
    /// 스킬 슬롯을 비울 때 UI 초기화
    /// </summary>
    /// <param name="index">초기화할 스킬 슬롯 인덱스</param>
    public void ClearSkillUI(int index)
    {
        if (index < 0 || index >= skillConditions.Length)
            return;

        skillConditions[index].ClearSkill();

        // UI 갱신
        skillConditions[index].RefreshUI();
    }

    public void UIUpdate(int index, float cooldown)
    {
        if (index < 0 || index >= skillConditions.Length) return;

        var condition = skillConditions[index];
        condition.currentCooldown = cooldown;

        // ✅ fillAmount도 여기서 직접 갱신하는 게 가장 안정적
        float cooldownReduction = Mathf.Clamp01(skillManager.player._playerStat.GetStatValue(PlayerStatType.SkillCooltime) / 100f);
        float realCooldown = condition.maximumCooldown * (1f - cooldownReduction);

        condition.UICoolDown.fillAmount = Mathf.Clamp01(cooldown / realCooldown);
    }
    public SkillCondition GetSlotCondition(int index)
    {
        if (index < 0 || index >= skillConditions.Length)
        {
            Debug.LogWarning($"[UISkill] 유효하지 않은 슬롯 인덱스: {index}");
            return null;
        }

        return skillConditions[index];
    }
}
