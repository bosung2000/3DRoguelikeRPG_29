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

    private void Start()
    {

    }
    private void Update()
    {
        
    }

    /// <summary>
    /// 바뀐 enabledSkills에 맞게 스킬 UI 변경
    /// </summary>
    public void ResetSkillUI(int index, EnabledSkills enabledSkills)
    {
        skillConditions[index].skill = enabledSkills.skill;
        skillConditions[index].ResetCondition();
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
        //skillConditions[index].RefreshUI();
    }

    public void UIUpdate(int index, float cooldown)
    {
        skillConditions[index].currentCooldown = cooldown;
    }
}
