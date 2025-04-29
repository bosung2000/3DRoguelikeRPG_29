using System.Collections.Generic;
using UnityEngine;

public class SkillFactory : MonoBehaviour
{
    //private static SkillFactory instance;
    //public static SkillFactory Instance
    //{
    //    get
    //    {
    //        if (instance == null)
    //        {
    //            GameObject go = new GameObject("SkillFactory");
    //            instance = go.AddComponent<SkillFactory>();
    //            DontDestroyOnLoad(go);
    //        }
    //        return instance;
    //    }
    //}

    private Dictionary<string, System.Type> skillTypes;

    private void Awake()
    {
        skillTypes = new Dictionary<string, System.Type>();
        // 스킬 타입 등록
        RegisterSkillType("Stab", typeof(StabSkill));
        RegisterSkillType("HalfMoon", typeof(HalfMoon));
        RegisterSkillType("OneArrow", typeof(OneArrow));
        
        // 여기에 다른 스킬들도 등록
    }

    public void RegisterSkillType(string skillName, System.Type skillType)
    {
        if (!skillTypes.ContainsKey(skillName))
        {
            skillTypes.Add(skillName, skillType);
        }
    }

    public MeleeSkillBase CreateMeleeSkill(Skill skillData)
    {
        if (skillTypes.TryGetValue(skillData._name, out System.Type skillType))
        {
            GameObject skillObj = new GameObject(skillData._name);
            MeleeSkillBase skill = (MeleeSkillBase)skillObj.AddComponent(skillType);
            skill.SetSkillData(skillData);
            
            // Enemy 레이어를 타겟으로 설정
            int enemyLayer = LayerMask.NameToLayer("Enemy");
            if (enemyLayer != -1)
            {
                skill.targetLayer = 1 << enemyLayer;
            }
            else
            {
                Debug.LogWarning("Enemy 레이어가 존재하지 않습니다. 타겟 레이어를 설정하세요.");
            }
            
            return skill;
        }
        return null;
    }

    public RangeSkillBase CreateRangeSkill(Skill skillData)
    {
        if (skillTypes.TryGetValue(skillData._name, out System.Type skillType))
        {
            GameObject skillObj = new GameObject(skillData._name);
            RangeSkillBase skill = (RangeSkillBase)skillObj.AddComponent(skillType);
            skill.SetSkillData(skillData);
            
            // Enemy 레이어를 타겟으로 설정
            int enemyLayer = LayerMask.NameToLayer("Enemy");
            if (enemyLayer != -1)
            {
                skill.SetTargetLayer(1 << enemyLayer);
            }
            else
            {
                Debug.LogWarning("Enemy 레이어가 존재하지 않습니다. 타겟 레이어를 설정하세요.");
            }
            
            return skill;
        }
        return null;
    }
} 