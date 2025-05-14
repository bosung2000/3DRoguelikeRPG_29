using System.Collections.Generic;
using UnityEngine;

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
    public bool _isBoolSkill { get; private set; } = false;
    public SkillInstance[] skillInstances
    {
        get;
        private set;
    }

    // ActiveSkill =지금 장착되어 있는 스킬 
    private Dictionary<int, SkillInstance> _activeSkills = new Dictionary<int, SkillInstance>();
    public Dictionary<int, SkillInstance> ActiveSkills
    {
        get { return _activeSkills; }
        private set { _activeSkills = value; }
    }

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
        ActiveSkills.Clear();

        // UI 슬롯 수만큼 스킬 초기화
        for (int i = 0; i < uiSkill.transform.childCount; i++)
        {
            if (i < skillInstances.Length)
            {
                var skillInstance = skillInstances[i];
                skillInstance.skillComponent = CreateSkillComponent(skillInstance.skill);
                //ActiveSkills[i] = skillInstance;
                //uiSkill.ResetSkillUI(i, skillInstance);
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
        foreach (var kvp in ActiveSkills)
        {
            int slotIndex = kvp.Key;
            SkillInstance skillInstance = kvp.Value;

            if (skillInstance.skill.cooldown > 0)
            {
                skillInstance.skill.cooldown -= Time.deltaTime;
                uiSkill.UIUpdate(slotIndex, skillInstance.skill.cooldown); // 슬롯 인덱스 사용
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

        if (HasThisSkill(skillIndex) || slotIndex < 0 || slotIndex >= uiSkill.transform.childCount)
        {
            Debug.Log($"스킬을 장착할 수 없습니다. 슬롯 인덱스: {slotIndex}, 유효 범위: 0-{uiSkill.transform.childCount - 1}");
            return false;
        }

        // 기존 슬롯의 스킬 제거
        if (ActiveSkills.ContainsKey(slotIndex))
        {
            //var oldSkill = ActiveSkills[slotIndex];
            //if (oldSkill.skillComponent != null)
            //{
            //    Destroy(oldSkill.skillComponent.gameObject);
            //}
            //기술 슬롯의 UI 제거 
            UnequipSkill(slotIndex);
        }

        // 새 스킬 설정
        var newSkill = skillInstances[skillIndex];
        newSkill.skillComponent = CreateSkillComponent(newSkill.skill);
        ActiveSkills[slotIndex] = newSkill;

        // UI 업데이트
        uiSkill.ResetSkillUI(slotIndex, newSkill);

        return true;
    }

    public void UnequipSkill(int slotIndex)
    {
        if (ActiveSkills.TryGetValue(slotIndex, out var skillInstance))
        {
            if (skillInstance.skillComponent != null)
            {
                Destroy(skillInstance.skillComponent.gameObject);
            }
            ActiveSkills.Remove(slotIndex);
            uiSkill.ClearSkillUI(slotIndex);
        }
        // UI 업데이트
        uiSkill.ResetSkillUI(slotIndex);
    }

    public Skill GetSkillAtSlot(int slotIndex)
    {
        if (ActiveSkills.TryGetValue(slotIndex, out var skillInstance))
        {
            return skillInstance.skill;
        }
        return null;
    }

    public bool HasSkillAtSlot(int slotIndex)
    {
        return ActiveSkills.ContainsKey(slotIndex);
    }

    public Skill _skill;
    public Vector3 _direction;
    public void InitSkillData(Skill skill, Vector3 direction)
    {
        _skill = skill;
        _direction = direction;
    }
    public bool IsCkSkill()
    {
        if (_skill == null)
        {
            Debug.LogError("스킬이 없습니다. 스킬을 장착했는지 확인하세요.");
            return false;
        }

        // 쿨다운 체크
        if (_skill.cooldown > 0)
        {
            Debug.Log($"스킬 {_skill._name}이(가) 쿨다운 중입니다. 남은 시간: {_skill.cooldown:F1}초");
            return false;
        }

        // 마나 체크
        if (player._playerStat.GetStatValue(PlayerStatType.MP) < _skill.requiredMana)
        {
            Debug.Log($"마나가 부족합니다. 필요: {_skill.requiredMana}, 현재: {player._playerStat.GetStatValue(PlayerStatType.MP)}");
            return false;
        }

        return true;
    }
    public void ActiveSkill()
    {
        // 방향 벡터 검증
        if (_direction == Vector3.zero)
        {
            _direction = player.transform.forward;
            if (_direction == Vector3.zero)
            {
                _direction = Vector3.forward;
            }
        }

        // 마나 소모
        player._playerStat.UseMana(_skill.requiredMana);

        // 스킬 실행
        foreach (var skillInstance in ActiveSkills.Values)
        {
            if (skillInstance.skill == _skill)
            {
                if (skillInstance.skillComponent is MeleeSkillBase meleeSkill)
                {
                    meleeSkill.Execute(player, _direction);
                }
                else if (skillInstance.skillComponent is RangeSkillBase rangeSkill)
                {
                    rangeSkill.Execute(player, _direction);
                }
                break;
            }
        }

        // 쿨타임 적용
        _skill.cooldown = _skill.maxCooldown * player._playerStat.GetStatValue(PlayerStatType.SkillColltime);

        // 스킬 사용 이벤트 발생
        //player.GetComponent<PlayerController>().SetTrigger("Skill");
    }
   

    internal int ReturnTotalSlotCount()
    {
        return skillInstances.Length;
    }
    public void SetActiveSkilltrue()
    {
        _isBoolSkill = true;
    }
    public void SetActiveSkillfalse()
    {
        _isBoolSkill = false;
    }
}

