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
    //private void FireProjectile(Vector2 _lookDirection, float angle)
    //{
    //    //투사체 관리자에서 사격 메서드 호출
    //    ShootBullet(
    //        this, //이 스크립트에서
    //              //투사체가 발사되는 지점을 기점으로
    //        RotateVector2(_lookDirection, angle) //lookdirection에서 angle만큼 회전시킨 값으로 발사하겠다
    //        );
    //}

    //public void ShootBullet(RangeWeaponHandler rangeWeaponHandler, Vector2 startPosition, Vector2 direction)
    //{
    //    //rangeWeaponHandler에 저장되어있는 BulletIndex의 투사체를 변수로 가져온 뒤 복제
    //    GameObject origin = projectilePrefabs[rangeWeaponHandler.BulletIndex];
    //    GameObject obj = Instantiate(origin, startPosition, Quaternion.identity);

    //    //그리고 투사체 안에 있는 투사체 제어자를 변수로 지정한 뒤 사격 지시
    //    ProjectileController projectileController = obj.GetComponent<ProjectileController>();
    //    projectileController.Init(direction, rangeWeaponHandler, this);
    //}

    //public void Init(Vector2 direction, RangeWeaponHandler weaponHandler, ProjectileManager projectileManager)
    //{
    //    //입력받은 매개변수를 저장하고 그 값에 맞춰서 조정하기
    //    this.projectileManager = projectileManager;
    //    rangeWeaponHandler = weaponHandler;
    //    this.direction = direction;
    //    currentDuration = 0;
    //    transform.localScale = Vector3.one * weaponHandler.BulletSize;
    //    spriteRenderer.color = weaponHandler.ProjectileColor;

    //    //오른쪽 축을 이 스크립트의 방향으로 설정하기
    //    transform.right = this.direction;

    //    if (direction.x < 0)
    //    {
    //        pivot.localRotation = Quaternion.Euler(180, 0, 0);
    //    }
    //    else
    //    {
    //        pivot.localRotation = Quaternion.Euler(0, 0, 0);
    //    }
    //    isReady = true;
    //}

    //private static Vector2 RotateVector2(Vector2 v, float degree)
    //{
    //    //쿼터니언의 각도만큼 벡터 회전시키기
    //    return Quaternion.Euler(0, 0, degree) * v;
    //}

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
    private SkillInstance[] SkillInstances;
    public EnabledSkills[] enabledSkills;

    private void Awake()
    {
        player= GetComponent<Player>();
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

        for(int i=0;i<skills.Length;i++)
        {
            SkillInstances[i].skill = skills[i];
            SkillInstances[i].index = i;
        }

        //스킬 ui가 표시할 수 있는 스킬 수만큼 스킬배열 길이를 정하고 반복문 시작
        enabledSkills = new EnabledSkills[uiSkill.transform.childCount];

        for (int i = 0; i < enabledSkills.Length; i++)
        {
            uiSkill.skillConditions[i].index = i;
            uiSkill.skillConditions[i].joystick.index = i;
            //enabledSkills의 고유 번호 지정 및 해당 enabledSkills로 UI 초기화
            enabledSkills[i] = new EnabledSkills();
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
    public void OnSkillClick(Skill skill, Vector3 direction)
    {
        //스킬이 공격중이지 않고 플레이어의 마나가 스킬의 마나보다 많다면
        //if (!skill.isAttacking && player.mana > skill.requiredMana())
        //{
        //    //공격 활성화시키고
        //    skill.isAttacking = true;
        //    if (!Skill)
        //    {
        //        //애니메이션 재생 이후 
        //        animator.SetTrigger("Skill");
        //        Skill = true;
        //    }
        //    else
        //    {
        //        animator.SetTrigger("Skill_Alternative");
        //        Skill = false;
        //    }
        //    // 이후 재사용 가능하게 attackRate초 뒤 활성화시키기
        //    Invoke(nameof(OnCanUseSkill), attackRate);
        //}
    }

    /// <summary>
    /// 스킬이 시전되고 나서, 시전 중이라는 불리언을 비활성화시키는 메서드
    /// </summary>
    void OnCanUseSkill(Skill skill)
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
