using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISkill : MonoBehaviour
{
    public SkillCondition[] skillConditions;
    public SkillManager skillManager;

    private void Awake()
    {
        skillConditions = new SkillCondition[this.transform.childCount];
        for (int i = 0; i < skillConditions.Length; i++)
        {
            skillConditions[i] = this.transform.GetChild(i).GetComponent<SkillCondition>(); //객체 안에서 SkillCondition 클래스를 찾아 지정하고 
            skillConditions[i].index = i; //고유 식별값을 지정하기
        }
    }

    private void Start()
    {
    }
}
