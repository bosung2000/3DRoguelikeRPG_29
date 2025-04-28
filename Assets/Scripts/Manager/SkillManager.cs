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
    private SkillInstance[] skillInstances; //스킬을 읽어온것이다(나중에는 많겠지 ?)
    public EnabledSkills[] enabledSkills; //사용가능한 개수만큼만 스킬슬롯에 넣기 
    
    // 실제로 활성화된 스킬만 관리하는 리스트
    private List<EnabledSkills> _activeSkills = new List<EnabledSkills>();

    private void Awake()
    {
        // Resources폴더의 /PlayerSkill 폴더의 모든 것을 가져와 배열로 만들고
        skills = Resources.LoadAll<Skill>("PlayerSkills");
        _camera = Camera.main;

        skillInstances = new SkillInstance[skills.Length];

        //스킬을 읽어와서 복재 
        for (int i = 0; i < skills.Length; i++)
        {
            skillInstances[i] = new SkillInstance() { index = i, skill = skills[i] };
        }

        // 스킬 ui가 표시할 수 있는 스킬 수만큼 스킬배열 길이를 정하고 반복문 시작
        enabledSkills = new EnabledSkills[uiSkill.transform.childCount];
    }

    private void Start()
    {
        // 활성 스킬 리스트 초기화
        _activeSkills.Clear();
        
        for (int i = 0; i < enabledSkills.Length; i++)
        {
            // 스킬 인스턴스가 범위를 벗어나면 건너뛰기
            if (i >= skillInstances.Length)
                continue;
            
            // 새 EnabledSkills 객체 생성
            enabledSkills[i] = new EnabledSkills() { index = i, skill = skillInstances[i].skill };
            
            // UI 업데이트
            uiSkill.ResetSkillUI(i, enabledSkills[i]);
            
            // 활성 스킬 리스트에 추가
            _activeSkills.Add(enabledSkills[i]);
            
            Debug.Log($" 착용 스킬 인덱스: {enabledSkills[i].index}, 현재 순환:{i}");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 활성화된 스킬만 쿨다운 감소 처리
        foreach (var activeSkill in _activeSkills)
        {
            if (activeSkill.skill.cooldown > 0)
            {
                activeSkill.skill.cooldown -= Time.deltaTime;
                
                // UI 업데이트
                int slotIndex = activeSkill.index;
                uiSkill.UIUpdate(slotIndex, activeSkill.skill.cooldown);
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

        // 기존 슬롯이 활성 스킬 리스트에 있었다면 제거
        if (enabledSkills[slotIndex] != null)
        {
            _activeSkills.Remove(enabledSkills[slotIndex]);
        }

        // 새 스킬 설정
        enabledSkills[slotIndex] = new EnabledSkills { index = slotIndex, skill = skills[skillIndex] };
        
        // 활성 스킬 리스트에 추가
        _activeSkills.Add(enabledSkills[slotIndex]);
        
        // UI 업데이트
        uiSkill.ResetSkillUI(slotIndex, enabledSkills[slotIndex]);

        return true;
    }

    /// <summary>
    /// 스킬 슬롯에서 스킬 제거
    /// </summary>
    /// <param name="slotIndex">스킬을 제거할 슬롯 인덱스</param>
    public void UnequipSkill(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= enabledSkills.Length)
            return;
            
        if (enabledSkills[slotIndex] != null)
        {
            // 활성 스킬 리스트에서 제거
            _activeSkills.Remove(enabledSkills[slotIndex]);
            
            // 슬롯에서 제거
            enabledSkills[slotIndex] = null;
            
            // UI 업데이트
            uiSkill.ClearSkillUI(slotIndex);
        }
    }

    /// <summary>
    /// 특정 슬롯의 스킬을 안전하게 가져옵니다.
    /// </summary>
    /// <param name="slotIndex">스킬 슬롯 인덱스</param>
    /// <returns>스킬 객체 (없으면 null)</returns>
    public Skill GetSkillAtSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= enabledSkills.Length)
            return null;
            
        return enabledSkills[slotIndex]?.skill;
    }
    
    /// <summary>
    /// 슬롯에 스킬이 있는지 확인합니다.
    /// </summary>
    /// <param name="slotIndex">스킬 슬롯 인덱스</param>
    /// <returns>스킬 존재 여부</returns>
    public bool HasSkillAtSlot(int slotIndex)
    {
        return GetSkillAtSlot(slotIndex) != null;
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
        // 원거리 스킬 로직 개선
        if (skill.projectilePrefabs != null)
        {
            // 방향 벡터 정규화 (필수)
            direction = direction.normalized;
            
            // 발사 위치 계산 (플레이어 위치에서 약간 앞으로)
            Vector3 spawnPosition = player.transform.position + direction * 1.0f;
            spawnPosition.y += 1.0f; // 바닥보다 약간 위에서 발사
            
            // 스킬 속도 검증 (0이하면 기본값 설정)
            if (skill.projectileSpeed <= 0)
            {
                skill.projectileSpeed = 10;
                Debug.LogWarning($"스킬 '{skill._name}'의 속도가 0 이하로 설정되어 있습니다. 기본값 10으로 설정합니다.");
            }
            
            // 발사 패턴에 따른 분기 처리
            switch (skill.castPattern)
            {
                case CastPattern.Burst:
                    // 연속 발사 패턴
                    StartCoroutine(BurstFire(skill, spawnPosition, direction, skill.burstCount, skill.burstDelay));
                    break;
                    
                case CastPattern.Spread:
                    // 부채꼴 다중 발사 패턴
                    SpreadFire(skill, spawnPosition, direction);
                    break;
                    
                case CastPattern.Rain:
                    // 대상 지점에 비처럼 내리는 발사체
                    StartCoroutine(ProjectileRain(skill, player.transform.position + direction * skill.attackRange, skill.rainCount));
                    break;
                    
                case CastPattern.Circle:
                    // 원형으로 퍼지는 발사체
                    CircleFire(skill, spawnPosition);
                    break;
                    
                case CastPattern.Single:
                default:
                    // 단일 발사체 생성 (기본 로직)
                    FireSingleProjectile(skill, spawnPosition, direction);
                    break;
            }
            
            // 발사 효과음 재생 (필요시)
            if (skill.soundEffectPrefab != null)
            {
                AudioSource.PlayClipAtPoint(skill.soundEffectPrefab, spawnPosition);
            }
            
            // 발사 이펙트 생성 (필요시)
            if (skill.effectPrefab != null)
            {
                GameObject effect = Instantiate(skill.effectPrefab, spawnPosition, Quaternion.LookRotation(direction));
                Destroy(effect, 2f);
            }
        }
        else
        {
            Debug.LogError($"스킬 '{skill._name}'의 발사체 프리팹이 설정되지 않았습니다.");
        }
    }
    
    // 단일 발사체 생성
    private void FireSingleProjectile(Skill skill, Vector3 spawnPosition, Vector3 direction)
    {
        // 방향은 항상 정규화된 벡터여야 합니다
        direction = direction.normalized;
        
        GameObject projectile = Instantiate(skill.projectilePrefabs, spawnPosition, Quaternion.LookRotation(direction));
        SkillProjectile projectileScript = projectile.GetComponent<SkillProjectile>();
        
        if (projectileScript != null)
        {
            // 공통 속성 설정 - 방향, 속도, 플레이어 참조 전달
            projectileScript.Init(direction, skill.projectileSpeed, player);
            
            // 데미지 설정 (플레이어 스탯 반영)
            float damageMultiplier = 1.0f + (player._playerStat.GetStatValue(PlayerStatType.Attack) * 0.01f);
            projectileScript.damage = Mathf.RoundToInt(skill.value * damageMultiplier);
            
            // 발사체 타입에 따른 속성 설정
            ConfigureProjectileByType(projectileScript, skill.projectileType);
            
            // 디버그용 로그
            Debug.Log($"발사체 생성: 속도={skill.projectileSpeed}, 방향={direction}");
        }
        else
        {
            Debug.LogError("발사체 프리팹에 SkillProjectile 컴포넌트가 없습니다!");
        }
    }
    
    // 부채꼴 발사 패턴
    private void SpreadFire(Skill skill, Vector3 spawnPosition, Vector3 direction)
    {
        int projectileCount = skill.spreadCount;
        float spreadAngle = skill.spreadAngle;
        
        // 시작 각도 계산 (중앙을 기준으로 대칭이 되도록)
        float startAngle = -spreadAngle * (projectileCount - 1) / 2;
        
        for (int i = 0; i < projectileCount; i++)
        {
            // 각도 계산
            float currentAngle = startAngle + spreadAngle * i;
            Vector3 rotatedDirection = Quaternion.Euler(0, currentAngle, 0) * direction;
            
            // 발사체 생성
            GameObject projectile = Instantiate(skill.projectilePrefabs, spawnPosition, Quaternion.LookRotation(rotatedDirection));
            SkillProjectile projectileScript = projectile.GetComponent<SkillProjectile>();
            
            if (projectileScript != null)
            {
                // 공통 속성 설정 - 플레이어 참조 추가
                projectileScript.Init(rotatedDirection, skill.projectileSpeed, player);
                
                // 데미지 설정 (플레이어 스탯 반영)
                float damageMultiplier = 1.0f + (player._playerStat.GetStatValue(PlayerStatType.Attack) * 0.01f);
                projectileScript.damage = Mathf.RoundToInt(skill.value * damageMultiplier);
                
                // 발사체 타입에 따른 속성 설정
                ConfigureProjectileByType(projectileScript, skill.projectileType);
            }
        }
    }
    
    // 원형 발사 패턴
    private void CircleFire(Skill skill, Vector3 spawnPosition)
    {
        int projectileCount = skill.circleCount;
        
        // 원 둘레를 따라 균등하게 배치
        float angleStep = 360f / projectileCount;
        
        for (int i = 0; i < projectileCount; i++)
        {
            // 각도 계산
            float angle = i * angleStep;
            Vector3 direction = new Vector3(
                Mathf.Sin(angle * Mathf.Deg2Rad),
                0,
                Mathf.Cos(angle * Mathf.Deg2Rad)
            ).normalized;
            
            // 발사체 생성
            GameObject projectile = Instantiate(skill.projectilePrefabs, spawnPosition, Quaternion.LookRotation(direction));
            SkillProjectile projectileScript = projectile.GetComponent<SkillProjectile>();
            
            if (projectileScript != null)
            {
                // 공통 속성 설정 - 플레이어 참조 추가
                projectileScript.Init(direction, skill.projectileSpeed, player);
                
                // 데미지 설정 (플레이어 스탯 반영)
                float damageMultiplier = 1.0f + (player._playerStat.GetStatValue(PlayerStatType.Attack) * 0.01f);
                projectileScript.damage = Mathf.RoundToInt(skill.value * damageMultiplier);
                
                // 발사체 타입에 따른 속성 설정
                ConfigureProjectileByType(projectileScript, skill.projectileType);
            }
        }
    }
    
    // 발사체 타입에 따른 속성 설정
    private void ConfigureProjectileByType(SkillProjectile projectile, ProjectileType type)
    {
        // ProjectileType에 따라 발사체의 특성 설정
        switch (type)
        {
            case ProjectileType.Penetrating:
                // 관통 속성 설정
                projectile.SetPenetrating(true, 3); // 기본 3번 관통
                break;
                
            case ProjectileType.Homing:
                // 유도 속성 설정
                projectile.SetHoming(true);
                break;
                
            case ProjectileType.Explosive:
                // 폭발 속성 설정
                projectile.SetSplashDamage(true);
                break;
                
            case ProjectileType.Chain:
                // 체인 속성 설정 - 구현 필요
                // 첫 대상 히트 후 가까운 다른 대상으로 튕기도록 구현
                break;
                
            case ProjectileType.Normal:
            default:
                // 기본 발사체는 특별한 설정 없음
                projectile.ConfigureProjectile(false, 0, false, false);
                break;
        }
    }
    
    // 연속 발사 코루틴
    private IEnumerator BurstFire(Skill skill, Vector3 spawnPosition, Vector3 direction, int count, float delay)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject projectile = Instantiate(skill.projectilePrefabs, spawnPosition, Quaternion.LookRotation(direction));
            SkillProjectile projectileScript = projectile.GetComponent<SkillProjectile>();
            
            if (projectileScript != null)
            {
                // 공통 속성 설정 - 플레이어 참조 추가
                projectileScript.Init(direction, skill.projectileSpeed, player);
                
                // 데미지 설정 (플레이어 스탯 반영)
                float damageMultiplier = 1.0f + (player._playerStat.GetStatValue(PlayerStatType.Attack) * 0.01f);
                projectileScript.damage = Mathf.RoundToInt(skill.value * damageMultiplier);
                
                // 발사체 타입에 따른 속성 설정
                ConfigureProjectileByType(projectileScript, skill.projectileType);
            }
            
            // 발사 효과음 재생 (필요시)
            if (skill.soundEffectPrefab != null)
            {
                AudioSource.PlayClipAtPoint(skill.soundEffectPrefab, spawnPosition);
            }
            
            yield return new WaitForSeconds(delay);
        }
    }
    
    // 범위 발사체 비 코루틴
    private IEnumerator ProjectileRain(Skill skill, Vector3 targetPosition, int count)
    {
        float radius = skill.rainRadius; // 비가 내릴 반경
        
        for (int i = 0; i < count; i++)
        {
            // 랜덤 위치 계산 (원형 영역 내)
            float angle = Random.Range(0f, 360f);
            float distance = Random.Range(0f, radius);
            Vector3 offset = new Vector3(
                Mathf.Cos(angle * Mathf.Deg2Rad) * distance,
                0f,
                Mathf.Sin(angle * Mathf.Deg2Rad) * distance
            );
            
            // 높은 위치에서 발사체 생성
            Vector3 spawnPosition = targetPosition + offset + Vector3.up * 10f;
            Vector3 direction = Vector3.down; // 아래 방향으로 발사
            
            GameObject projectile = Instantiate(skill.projectilePrefabs, spawnPosition, Quaternion.LookRotation(direction));
            SkillProjectile projectileScript = projectile.GetComponent<SkillProjectile>();
            
            if (projectileScript != null)
            {
                // 공통 속성 설정 - 플레이어 참조 추가
                projectileScript.Init(direction, skill.projectileSpeed, player);
                
                // 데미지 설정 (플레이어 스탯 반영)
                float damageMultiplier = 1.0f + (player._playerStat.GetStatValue(PlayerStatType.Attack) * 0.01f);
                projectileScript.damage = Mathf.RoundToInt(skill.value * damageMultiplier);
                
                // 발사체 타입에 따른 속성 설정
                if (skill.projectileType == ProjectileType.Explosive)
                {
                    projectileScript.SetSplashDamage(true);
                }
            }
            
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void CastBuffSkill(Skill skill)
    {
        // 버프 스킬 로직
        switch (skill.buffType)
        {
            case BuffType.Heal:
                // 힐링 효과
                //player.GetComponent<PlayerStat>().Heal(skill.value);
                break;

            case BuffType.ATK:
                // 공격력 증가
                // player.GetComponent<PlayerStat>().AddAttackBuff(skill.value, 10f); // 10초 동안 버프
                break;

            case BuffType.RES:
                // 방어력 증가
                //player.GetComponent<PlayerStat>().AddDefenseBuff(skill.value, 10f); // 10초 동안 버프
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

