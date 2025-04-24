using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class EnabledSkills
{
    public Skill skill;
    public int index;
}

public class SkillInstance
{
    public Skill skill;
    public int index;
}

public class SkillManager : MonoBehaviour
{
    public Player player;
    public UISkill uiSkill;
    public bool attacking;
    public float attackRate;
    private Camera _camera;
    private Skill[] skills;
    private SkillInstance[] skillInstances;
    public EnabledSkills[] enabledSkills;

    // Start is called before the first frame update
    void Start()
    {
        // Resources폴더의 /PlayerSkill 폴더의 모든 것을 가져와 배열로 만들고
        skills = Resources.LoadAll<Skill>("PlayerSkills");
        _camera = Camera.main;

        skillInstances = new SkillInstance[skills.Length];

        for (int i = 0; i < skills.Length; i++)
        {
            skillInstances[i] = new SkillInstance();
            skillInstances[i].skill = skills[i];
            skillInstances[i].index = i;
        }

        // 스킬 ui가 표시할 수 있는 스킬 수만큼 스킬배열 길이를 정하고 반복문 시작
        enabledSkills = new EnabledSkills[uiSkill.transform.childCount];

        for (int i = 0; i < enabledSkills.Length; i++)
        {
            enabledSkills[i] = new EnabledSkills();
            enabledSkills[i].skill = skillInstances[0].skill;
            uiSkill.skillConditions[i].index = i;
            uiSkill.skillConditions[i].joystick.index = i;
            enabledSkills[i].index = i;
            ResetSkillUI(i);
            Debug.Log($"조이스틱 인덱스:{uiSkill.skillConditions[i].joystick.index}, 착용 스킬 인덱스: {enabledSkills[i].index}, 현재 순환:{i}");
        }
    }

    /// <summary>
    /// 바뀐 enabledSkills에 맞게 스킬 UI 변경
    /// </summary>
    public void ResetSkillUI(int index)
    {
        uiSkill.skillConditions[index].skill = enabledSkills[index].skill;
        uiSkill.skillConditions[index].ResetCondition();
    }

    // Update is called once per frame
    void Update()
    {
        // 스킬 쿨다운 감소
        for (int i = 0; i < enabledSkills.Length; i++)
        {
            if (enabledSkills[i].skill != null && enabledSkills[i].skill.cooldown > 0)
            {
                enabledSkills[i].skill.cooldown -= Time.deltaTime;
                // UI 업데이트
                uiSkill.skillConditions[i].currentCooldown = enabledSkills[i].skill.cooldown;
            }
        }
    }

    /// <summary>
    /// 스킬을 배우는 메서드
    /// </summary>
    /// <param name="index"></param>
    public bool LearnSkill(int index)
    {
        // 스킬을 갖고 있는지 확인하고, 만약 있다면
        if (HasThisSkill(index))
        {
            // 아무 일도 일어나지 않음
            Debug.Log("이미 갖고 있는 스킬입니다.");
            return false;
        }
        else // 만약 없다면
        {
            // 그 스킬의 isOwned 값을 활성화
            skills[index].isOwned = true;
            return true;
        }
    }

    /// <summary>
    /// 매개변수 값 번째의 skill이 현재 갖고 있는 스킬인지(isOwned) 확인하는 메서드
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public bool HasThisSkill(int index)
    {
        if (skills[index].isOwned)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 스킬 슬롯에 특정 스킬을 장착하는 메서드
    /// </summary>
    /// <param name="skillIndex">장착할 스킬 인덱스</param>
    /// <param name="slotIndex">장착할 슬롯 인덱스</param>
    /// <returns>성공 여부</returns>
    public bool EquipSkill(int skillIndex, int slotIndex)
    {
        if (!HasThisSkill(skillIndex) || slotIndex >= enabledSkills.Length)
        {
            Debug.Log("스킬을 장착할 수 없습니다.");
            return false;
        }

        enabledSkills[slotIndex].skill = skills[skillIndex];
        enabledSkills[slotIndex].index = skillIndex;
        ResetSkillUI(slotIndex);

        return true;
    }

    /// <summary>
    /// 스킬을 눌렀을 때, 스킬 발동 메서드
    /// </summary>
    public void OnSkillClick(Skill skill, Vector3 direction)
    {
        if (skill == null)
        {
            Debug.Log("스킬이 없습니다.");
            return;
        }

        // 쿨다운 체크
        if (skill.cooldown > 0)
        {
            Debug.Log("스킬이 쿨다운 중입니다.");
            return;
        }

        // 마나 체크
        if (player._playerStat.GetStatValue(PlayerStatType.MP) < skill.requiredMana)
        {
            Debug.Log("마나가 부족합니다.");
            return;
        }

        // 마나 소모
        player.GetComponent<PlayerStat>().UseMana(skill.requiredMana);

        // 스킬 타입에 따라 처리
        switch (skill.skillType)
        {
            case SkillType.Melee:
                // 근접 공격 로직
                CastMeleeSkill(skill, direction);
                break;

            case SkillType.Ranged:
                // 원거리 공격 로직
                CastRangedSkill(skill, direction);
                break;

            case SkillType.Buff:
                // 버프 로직
                CastBuffSkill(skill);
                break;
        }

        // 쿨타임 적용
        skill.cooldown = skill.maxCooldown;

        // 스킬 사용 이벤트 발생 (애니메이션 등)
        player.GetComponent<PlayerController>().SetTrigger("Skill");
    }

    private void CastMeleeSkill(Skill skill, Vector3 direction)
    {
        // 근접 스킬 로직
        Vector3 center = player.transform.position;
        Vector3 halfExtents = new Vector3(skill.attackRange, 2f, skill.attackRange);
        Quaternion orientation = Quaternion.LookRotation(direction);

        // 부채꼴 범위 공격을 위한 각도 계산
        float angle = 90f; // 90도 부채꼴
        Vector3 forward = direction;

        // 범위 내 적 탐색
        Collider[] hitColliders = Physics.OverlapBox(center + forward * skill.attackRange * 0.5f,
                                                    halfExtents * 0.5f,
                                                    orientation,
                                                    LayerMask.GetMask("Enemy"));

        foreach (var hitCollider in hitColliders)
        {
            // 각도 체크 (부채꼴 범위 내에 있는지)
            Vector3 dirToTarget = (hitCollider.transform.position - center).normalized;
            float angleToTarget = Vector3.Angle(forward, dirToTarget);

            if (angleToTarget <= angle * 0.5f)
            {
                // 적에게 데미지 적용
                Enemy enemy = hitCollider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(skill.value);
                }
            }
        }

        // 이펙트 생성 (필요시)
        if (skill.effectPrefab != null)
        {
            GameObject effect = Instantiate(skill.effectPrefab,
                center + forward * skill.attackRange * 0.5f,
                Quaternion.LookRotation(direction));
            Destroy(effect, 2f);
        }
    }

    private void CastRangedSkill(Skill skill, Vector3 direction)
    {
        // 원거리 스킬 로직
        if (skill.projectilePrefabs != null)
        {
            skill.projectilePrefabs.GetComponent<SkillProjectile>().ProjectileSpeed = skill.projectileSpeed;
            skill.projectilePrefabs.GetComponent<SkillProjectile>().ShootBullet(player.transform.position, direction);
        }
    }

    private void CastBuffSkill(Skill skill)
    {
        // 버프 스킬 로직
        switch (skill.buffType)
        {
            case BuffType.Heal:
                // 힐링 효과
                player.GetComponent<PlayerStat>().Heal(skill.value);
                break;

            case BuffType.ATK:
                // 공격력 증가
                player.GetComponent<PlayerStat>().AddAttackBuff(skill.value, 10f); // 10초 동안 버프
                break;

            case BuffType.RES:
                // 방어력 증가
                player.GetComponent<PlayerStat>().AddDefenseBuff(skill.value, 10f); // 10초 동안 버프
                break;

            default:
                break;
        }

        // 버프 이펙트 생성 (필요시)
        if (skill.effectPrefab != null)
        {
            GameObject effect = Instantiate(skill.effectPrefab, player.transform.position, Quaternion.identity);
            effect.transform.SetParent(player.transform);
            Destroy(effect, 10f); // 10초 후 이펙트 제거
        }
    }
}

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//public class EnabledSkills
//{
//    public Skill skill;
//    public int index;
//}

//public class SkillInstance
//{
//    public Skill skill;
//    public int index;
//}

//public class SkillManager : MonoBehaviour
//{
//    public Player player;
//    public UISkill uiSkill;
//    public bool attacking;
//    public float attackRate;
//    private Camera _camera; 
//    private Skill[] skills;
//    private SkillInstance[] skillInstances;
//    public EnabledSkills[] enabledSkills;

//    // Start is called before the first frame update
//    void Start()
//    {
//        //Resources폴더의 /PlayerSkill 폴더의 모든 것을 가져와 배열로 만들고
//        skills = Resources.LoadAll<Skill>("PlayerSkills");


//        skillInstances=new SkillInstance[skills.Length];

//        for(int i=0;i<skills.Length;i++)
//        {
//            skillInstances[i] = new SkillInstance();
//            skillInstances[i].skill = skills[i];
//            skillInstances[i].index = i;
//        }

//        //스킬 ui가 표시할 수 있는 스킬 수만큼 스킬배열 길이를 정하고 반복문 시작
//        enabledSkills = new EnabledSkills[uiSkill.transform.childCount];

//        for (int i = 0; i < enabledSkills.Length; i++)
//        {
//            enabledSkills[i]=new EnabledSkills();
//            enabledSkills[i].skill=skillInstances[0].skill;
//            uiSkill.skillConditions[i].index = i;
//            uiSkill.skillConditions[i].joystick.index = i;
//            enabledSkills[i].index = i;
//            ResetSkillUI(i);
//            Debug.Log($"조이스틱 인덱스:{uiSkill.skillConditions[i].joystick.index}, 착용 스킬 인덱스: {enabledSkills[i].index}, 현재 순환:{i}");
//        }
//    }
//    /// <summary>
//    /// 바뀐 enabledSkills에 맞게 스킬 UI 변경
//    /// </summary>
//    public void ResetSkillUI(int index)
//    {
//        uiSkill.skillConditions[index].skill = enabledSkills[index].skill;
//        uiSkill.skillConditions[index].ResetCondition();
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        foreach(var skill in skills)
//        {
//            skill.cooldown -= Time.deltaTime;
//        }
//    }

//    /// <summary>
//    /// 스킬을 배우는 메서드
//    /// </summary>
//    /// <param name="index"></param>
//    public bool LearnSkill(int index)
//    {
//        //스킬을 갖고 있는지 확인하고, 만약 있다면
//        if (HasThisSkill(index))
//        {
//            //아무 일도 일어나지 않음
//            Debug.Log("이미 갖고 있는 스킬입니다.");
//            return false;
//        }
//        else //만약 없다면
//        {
//            //그 스킬의 isOwned 값을 활성화
//            skills[index].isOwned = true;
//            return true;
//        }
//    }
//    /// <summary>
//    /// 매개변수 값 번째의 skill이 현재 갖고 있는 스킬인지(isOwned) 확인하는 메서드
//    /// </summary>
//    /// <param name="index"></param>
//    /// <returns></returns>
//    public bool HasThisSkill(int index)
//    {
//        if (skills[index].isOwned)
//        {
//            return true;
//        }
//        else
//        {
//            return false;
//        }
//    }





//    /// <summary>
//    /// 스킬을 눌렀을 때, 스킬 애니메이션이 시전되며 시전 중임을 알리는 불리언을 활성화키는 메서드
//    /// 스킬의 적중은 애니메이션 진행 도중 OnSkillHit 메서드를 발동시킴으로써 적용시킬 것
//    /// </summary>
//    public void OnSkillClick(Skill skill, Vector3 direction)
//    {
//        //스킬이 공격중이지 않고 플레이어의 마나가 스킬의 마나보다 많다면
//        //if (!skill.isAttacking && player.mana > skill.requiredMana())
//        //{
//        //    //공격 활성화시키고
//        //    skill.isAttacking = true;
//        //    if (!Skill)
//        //    {
//        //        //애니메이션 재생 이후 
//        //        animator.SetTrigger("Skill");
//        //        Skill = true;
//        //    }
//        //    else
//        //    {
//        //        animator.SetTrigger("Skill_Alternative");
//        //        Skill = false;
//        //    }
//        //    // 이후 재사용 가능하게 attackRate초 뒤 활성화시키기
//        //    Invoke(nameof(OnCanUseSkill), attackRate);
//        //}
//    }

//    /// <summary>
//    /// 스킬이 시전되고 나서, 시전 중이라는 불리언을 비활성화시키는 메서드
//    /// </summary>
//    void OnCanUseSkill(Skill skill)
//    {
//        attacking = false;
//    }

//    /// <summary>
//    /// 실행될 애니메이션 클립 안에서 호출될 공격 메서드
//    /// </summary>
//    public void onSkillHit()
//    {
//        //Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
//        //Debug.DrawRay(ray.origin, ray.direction, Color.white);
//        //RaycastHit hit;

//        //condition.ConsumeStamina(attackStamina);

//        //if (Physics.Raycast(ray, out hit, attackDistance, hitLayer))
//        //{
//        //    Debug.Log(hit.collider.name);
//        //    if (hit.collider.TryGetComponent(out IBreakableObject breakbleObject))
//        //    {
//        //        Debug.Log("실행");
//        //        breakbleObject.TakeDamage(nowDamage);
//        //    }
//        //}
//    }

//}

