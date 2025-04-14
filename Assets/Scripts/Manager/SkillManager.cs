using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnabledSkills
{
    public Skill skill;
    public int index;
}
public class SkillManager : MonoBehaviour
{
    private static SkillManager instance;

    public static SkillManager Instance { get; set; }

    public Player player;
    public UISkill uiSkill;
    public bool attacking;
    public float attackRate;
    private Camera _camera; 
    private Skill[] skills;
    public EnabledSkills[] enabledSkills;

    private void Awake()
    {
        if (instance != null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (instance != null)
            {
                Destroy(gameObject);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Resources폴더의 /PlayerSkill 폴더의 모든 것을 가져와 배열로 만들고
        skills = Resources.LoadAll<Skill>("/PlayerSkills");

        //스킬 ui가 표시할 수 있는 스킬 수만큼 스킬배열 길이를 정하고 반복문 시작
        enabledSkills = new EnabledSkills[uiSkill.transform.childCount];

        for (int i = 0; i < enabledSkills.Length; i++)
        {
            //enabledSkills의 고유 번호 지정 및 해당 enabledSkills로 UI 초기화
            enabledSkills[i] = new EnabledSkills();
            enabledSkills[i].index = i;
            ResetSkillUI(i);
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
        foreach(var skill in skills)
        {
            skill.cooldown -= Time.deltaTime;
        }
    }

    /// <summary>
    /// 스킬을 배우는 메서드
    /// </summary>
    /// <param name="index"></param>
    public bool LearnSkill(int index)
    {
        //스킬을 갖고 있는지 확인하고, 만약 있다면
        if (HasThisSkill(index))
        {
            //아무 일도 일어나지 않음
            Debug.Log("이미 갖고 있는 스킬입니다.");
            return false;
        }
        else //만약 없다면
        {
            //그 스킬의 isOwned 값을 활성화
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
    /// 스킬을 눌렀을 때, 스킬 애니메이션이 시전되며 시전 중임을 알리는 불리언을 활성화키는 메서드
    /// 스킬의 적중은 애니메이션 진행 도중 OnSkillHit 메서드를 발동시킴으로써 적용시킬 것
    /// </summary>
    public void OnSkillClick(Skill skill)
    {

        //if (!skill.isAttacking && player.mana>skill.requiredMana())
        //{
        //    skill.isAttacking = true;
        //    if (!Skill)
        //    {
        //        animator.SetTrigger("Skill");
        //        Skill = true;
        //    }
        //    else
        //    {
        //        animator.SetTrigger("Skill_Alternative");
        //        Skill = false;
        //    }
        //    Invoke(nameof(OnCanUseSkill), attackRate);
        //}
    }

    /// <summary>
    /// 스킬이 시전되고 나서, 시전 중이라는 불리언을 비활성화시키는 메서드
    /// </summary>
    void OnCanUseSkill()
    {
        attacking = false;
    }

    /// <summary>
    /// 실행될 애니메이션 클립 안에서 호출될 공격 메서드
    /// </summary>
    public void onSkillHit()
    {
        //Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        //Debug.DrawRay(ray.origin, ray.direction, Color.white);
        //RaycastHit hit;

        //condition.ConsumeStamina(attackStamina);

        //if (Physics.Raycast(ray, out hit, attackDistance, hitLayer))
        //{
        //    Debug.Log(hit.collider.name);
        //    if (hit.collider.TryGetComponent(out IBreakableObject breakbleObject))
        //    {
        //        Debug.Log("실행");
        //        breakbleObject.TakeDamage(nowDamage);
        //    }
        //}
    }

}
