using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SkillInstance
{
    public Skill skill;
    public int index;
    public MonoBehaviour skillComponent; // 실제 스킬 컴포넌트
}

public class SkillManager : MonoBehaviour
{
    public Player player;
    public UISkill uiSkill;
    private Skill[] skills;
    private SkillInstance[] skillInstances;
    private Dictionary<int, SkillInstance> activeSkills = new Dictionary<int, SkillInstance>(); // 활성화된 스킬들

    private SkillFactory skillFactory;
    private void Awake()
    {
        skillFactory = GetComponent<SkillFactory>();
        // Resources폴더의 /PlayerSkill 폴더의 모든 것을 가져와 배열로 만들고
        skills = Resources.LoadAll<Skill>("PlayerSkills");
        skillInstances = new SkillInstance[skills.Length];

        //스킬을 읽어와서 복재 
        for (int i = 0; i < skills.Length; i++)
        {
            skillInstances[i] = new SkillInstance() { index = i, skill = skills[i] };
        }
    }

    private void Start()
    {
        // 활성 스킬 리스트 초기화
        activeSkills.Clear();

        // UI 슬롯 수만큼 스킬 초기화
        for (int i = 0; i < uiSkill.transform.childCount; i++)
        {
            if (i < skillInstances.Length)
            {
                var skillInstance = skillInstances[i];
                skillInstance.skillComponent = CreateSkillComponent(skillInstance.skill);
                activeSkills[i] = skillInstance;
                uiSkill.ResetSkillUI(i, skillInstance);
            }
        }
    }

    private MonoBehaviour CreateSkillComponent(Skill skill)
    {
        switch (skill.skillType)
        {
            case SkillType.Melee:
                return skillFactory.CreateMeleeSkill(skill);
            case SkillType.Ranged:
                return skillFactory.CreateRangeSkill(skill);
            case SkillType.Buff:
                // 버프 스킬 구현 필요
                return null;
            default:
                return null;
        }
    }

    void Update()
    {
        // 활성화된 스킬만 쿨다운 감소 처리
        foreach (var skillInstance in activeSkills.Values)
        {
            if (skillInstance.skill.cooldown > 0)
            {
                skillInstance.skill.cooldown -= Time.deltaTime;
                uiSkill.UIUpdate(skillInstance.index, skillInstance.skill.cooldown);
            }
        }
    }

    public bool LearnSkill(int index)
    {
        if (HasThisSkill(index))
        {
            Debug.Log("이미 갖고 있는 스킬입니다.");
            return false;
        }
        else
        {
            skills[index].isOwned = true;
            return true;
        }
    }

    public bool HasThisSkill(int index)
    {
        return skills[index].isOwned;
    }

    public bool EquipSkill(int skillIndex, int slotIndex)
    {
        if (!HasThisSkill(skillIndex) || slotIndex >= uiSkill.transform.childCount)
        {
            Debug.Log("스킬을 장착할 수 없습니다.");
            return false;
        }

        // 기존 슬롯의 스킬 제거
        if (activeSkills.ContainsKey(slotIndex))
        {
            var oldSkill = activeSkills[slotIndex];
            if (oldSkill.skillComponent != null)
            {
                Destroy(oldSkill.skillComponent.gameObject);
            }
        }

        // 새 스킬 설정
        var newSkill = skillInstances[skillIndex];
        newSkill.skillComponent = CreateSkillComponent(newSkill.skill);
        activeSkills[slotIndex] = newSkill;

        // UI 업데이트
        uiSkill.ResetSkillUI(slotIndex, newSkill);

        return true;
    }

    public void UnequipSkill(int slotIndex)
    {
        if (activeSkills.TryGetValue(slotIndex, out var skillInstance))
        {
            if (skillInstance.skillComponent != null)
            {
                Destroy(skillInstance.skillComponent.gameObject);
            }
            activeSkills.Remove(slotIndex);
            uiSkill.ClearSkillUI(slotIndex);
        }
    }

    public Skill GetSkillAtSlot(int slotIndex)
    {
        if (activeSkills.TryGetValue(slotIndex, out var skillInstance))
        {
            return skillInstance.skill;
        }
        return null;
    }

    public bool HasSkillAtSlot(int slotIndex)
    {
        return activeSkills.ContainsKey(slotIndex);
    }

    public void OnSkillClick(Skill skill, Vector3 direction)
    {
        if (skill == null)
        {
            Debug.LogError("스킬이 없습니다. 스킬을 장착했는지 확인하세요.");
            return;
        }

        // 쿨다운 체크
        if (skill.cooldown > 0)
        {
            Debug.Log($"스킬 {skill._name}이(가) 쿨다운 중입니다. 남은 시간: {skill.cooldown:F1}초");
            return;
        }

        // 마나 체크
        if (player._playerStat.GetStatValue(PlayerStatType.MP) < skill.requiredMana)
        {
            Debug.Log($"마나가 부족합니다. 필요: {skill.requiredMana}, 현재: {player._playerStat.GetStatValue(PlayerStatType.MP)}");
            return;
        }

        // 방향 벡터 검증
        if (direction == Vector3.zero)
        {
            direction = player.transform.forward;
            if (direction == Vector3.zero)
            {
                direction = Vector3.forward;
            }
        }

        // 마나 소모
        player._playerStat.UseMana(skill.requiredMana);

        // 스킬 실행
        foreach (var skillInstance in activeSkills.Values)
        {
            if (skillInstance.skill == skill)
            {
                if (skillInstance.skillComponent is MeleeSkillBase meleeSkill)
                {
                    meleeSkill.Execute(player, direction);
                }
                else if (skillInstance.skillComponent is RangeSkillBase rangeSkill)
                {
                    rangeSkill.Execute(player, direction);
                }
                break;
            }
        }

        // 쿨타임 적용
        skill.cooldown = skill.maxCooldown;

        // 스킬 사용 이벤트 발생
        player.GetComponent<PlayerController>().SetTrigger("Skill");
    }
}

