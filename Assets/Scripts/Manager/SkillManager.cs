using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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
    private event Action<Skill> OnSkillUsed;

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
        // 디버그 로그 추가
        Debug.Log($"OnSkillClick 호출됨: 스킬={skill?.name ?? "null"}, 방향={direction}, 벡터길이={direction.magnitude}");
        
        if (skill == null)
        {
            Debug.LogError("스킬이 없습니다. 스킬을 장착했는지 확인하세요.");
            return;
        }

        // 쿨다운 체크
        if (skill.cooldown > 0)
        {
            Debug.Log($"스킬 {skill.name}이(가) 쿨다운 중입니다. 남은 시간: {skill.cooldown:F1}초");
            return;
        }

        // 마나 체크
        if (player._playerStat.GetStatValue(PlayerStatType.MP) < skill.requiredMana)
        {
            Debug.Log($"마나가 부족합니다. 필요: {skill.requiredMana}, 현재: {player._playerStat.GetStatValue(PlayerStatType.MP)}");
            return;
        }
        
        // 방향 벡터 검증 - 모든 스킬에 공통 적용
        if (direction == Vector3.zero)
        {
            // 플레이어가 바라보는 방향을 기본값으로 사용
            direction = player.transform.forward;
            
            // 여전히 제로 벡터라면 기본 방향(앞쪽)으로 설정
            if (direction == Vector3.zero)
            {
                direction = Vector3.forward;
            }
            
        }

        
        // 마나 소모
        player._playerStat.UseMana(skill.requiredMana);

        // 스킬 타입에 따라 처리
        switch (skill.skillType)
        {
            case SkillType.Melee:
                // 근접 공격 로직
                Debug.Log($"근접 공격 스킬 발동: {skill.name}, 범위={skill.attackRange}");
                CastMeleeSkill(skill, direction);
                break;

            case SkillType.Ranged:
                // 원거리 공격 로직
                Debug.Log($"원거리 공격 스킬 발동: {skill.name}, 투사체={skill.projectilePrefabs != null}");
                CastRangedSkill(skill, direction);
                break;

            case SkillType.Buff:
                // 버프 로직
                Debug.Log($"버프 스킬 발동: {skill.name}, 버프타입={skill.buffType}");
                CastBuffSkill(skill);
                break;
                
            default:
                Debug.LogError($"알 수 없는 스킬 타입: {skill.skillType}");
                break;
        }

        // 쿨타임 적용
        skill.cooldown = skill.maxCooldown;

        // 스킬 사용 이벤트 발생 (애니메이션 등)
        player.GetComponent<PlayerController>().SetTrigger("Skill");
        
        // 스킬 이벤트 발생 (UI 업데이트 등)
        OnSkillUsed?.Invoke(skill);
    }

    private void CastMeleeSkill(Skill skill, Vector3 direction)
    {
        // 방향 벡터 정규화
        direction = direction.normalized;
        
        // 근접 스킬 로직
        Vector3 center = player.transform.position;
        
        // 스킬 범위 설정
        float attackRange = skill.attackRange;
        
        // 부채꼴 범위 공격을 위한 각도 계산
        float angle = 90f; // 90도 부채꼴
        Vector3 forward = direction;
        Debug.Log($"부채꼴 범위 설정: 각도={angle}도, 전방벡터={forward}");

        // 스킬 범위 시각화 (게임 내에서 실제로 보이는 효과)
        ShowMeleeSkillRange(center, forward, attackRange, angle);
        Debug.Log("스킬 범위 시각화 완료");

        // 박스 감지를 위한 설정
        // 플레이어 위치에서 스킬 범위의 절반 거리만큼 앞으로 이동한 위치를 박스 중심으로 설정
        Vector3 searchCenter = center + forward * (attackRange * 0.5f);
        
        // 박스 크기 설정 - 반드시 반쪽 크기로 지정
        Vector3 halfExtents = new Vector3(attackRange * 0.5f, 1f, attackRange * 0.5f);
        
        // 박스 회전 설정
        Quaternion orientation = Quaternion.LookRotation(direction);
        
        // 범위 내 적 탐색
        Collider[] hitColliders = Physics.OverlapBox(searchCenter,
                                                   halfExtents,
                                                   orientation,
                                                   LayerMask.GetMask("Enemy"));
                                                   
        Debug.Log($"감지된 적의 수: {hitColliders.Length}");
        Debug.Log($"박스캐스트 위치: {searchCenter}, 크기: {halfExtents}");

        int enemiesHit = 0;
        foreach (var hitCollider in hitColliders)
        {
            // 각도 체크 (부채꼴 범위 내에 있는지)
            Vector3 dirToTarget = (hitCollider.transform.position - center).normalized;
            float angleToTarget = Vector3.Angle(forward, dirToTarget);

            if (angleToTarget <= angle * 0.5f) // 부채꼴 각도의 절반과 비교해야 함
            {
                // 적에게 데미지 적용
                Enemy enemy = hitCollider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    Debug.Log($"데미지 적용: {hitCollider.name}에게 {skill.value} 데미지");
                    enemy.TakeDamage(skill.value);
                    enemiesHit++;
                    
                    // 히트 이펙트 생성
                    ShowHitEffect(enemy.transform.position);
                }
            }
        }

        // 이펙트 생성 (필요시)
        if (skill.effectPrefab != null)
        {
            // 이펙트 위치 플레이어 앞으로 조정 - 플레이어 바로 앞에서 시작하도록 수정
            Vector3 effectPosition = center + forward * (attackRange * 0.5f);
            effectPosition.y += 0.5f; // 약간 위로 올려서 바닥에 묻히지 않게
            
            GameObject effect = Instantiate(skill.effectPrefab,
                effectPosition,
                Quaternion.LookRotation(direction));
                
            // 이펙트 크기 증가 (더 잘 보이게)
            effect.transform.localScale = effect.transform.localScale * 1.5f;
            
            Destroy(effect, 2f);
        }
        
        // Gizmos 범위 표시 정보 업데이트
        lastMeleeCenter = center;
        lastMeleeDirection = forward;
        lastMeleeRange = attackRange;
        lastMeleeAngle = angle;
        showLastMeleeRange = true;
        
        // 5초 후 Gizmos 표시 숨김
        StartCoroutine(HideGizmosAfterDelay(5f));
    }
    
    // 근접 스킬 범위 시각화 (게임 내에서 보이는 효과)
    private void ShowMeleeSkillRange(Vector3 center, Vector3 direction, float range, float angle)
    {
        // 일시적인 바닥 범위 표시기 생성
        GameObject rangeIndicator = new GameObject("MeleeSkillRangeIndicator");
        
        // 플레이어 바로 앞에서 시작하여 범위의 중간 지점에 지시자 배치
        Vector3 indicatorPosition = center + direction * (range * 0.5f);
        indicatorPosition.y = 0.1f; // 바닥 위 약간 띄움
        rangeIndicator.transform.position = indicatorPosition;
        
        // 방향에 맞춰 회전
        rangeIndicator.transform.rotation = Quaternion.LookRotation(direction);
        
        // 메시 생성 시 원점을 조정하여 플레이어 위치에서 시작하도록 설정
        MeshFilter meshFilter = rangeIndicator.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = rangeIndicator.AddComponent<MeshRenderer>();
        
        // 반투명 재질 설정
        Material material = new Material(Shader.Find("Standard"));
        material.color = new Color(1f, 0.3f, 0f, 0.5f); // 주황색 반투명
        meshRenderer.material = material;
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        meshRenderer.receiveShadows = false;
        
        // 메시 생성 (플레이어 위치에서 시작하도록 조정된 부채꼴)
        Mesh mesh = CreateArcMesh(range, angle, true); // 플레이어 시작점 파라미터 추가
        meshFilter.mesh = mesh;
        
        // 잠시 후 제거
        Destroy(rangeIndicator, 0.7f);
        
        // 범위 외곽선 효과 추가 (플레이어 위치부터 시작하도록)
        StartCoroutine(CreateRangeOutline(center, direction, range, angle));
    }
    
    // 부채꼴 메시 생성
    private Mesh CreateArcMesh(float radius, float angle, bool startFromPlayer = false)
    {
        Mesh mesh = new Mesh();
        
        int segments = 30;
        float halfAngle = angle * 0.5f * Mathf.Deg2Rad;
        
        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3];
        
        // 중앙 정점 - 플레이어 위치에서 시작하도록 조정
        vertices[0] = startFromPlayer ? new Vector3(0, 0, -radius * 0.5f) : Vector3.zero;
        
        // 원호 정점들 조정
        for (int i = 0; i <= segments; i++)
        {
            float segmentAngle = -halfAngle + ((float)i / segments) * 2 * halfAngle;
            float x = Mathf.Sin(segmentAngle) * radius;
            float z = Mathf.Cos(segmentAngle) * radius;
            
            if (startFromPlayer)
                z -= radius * 0.5f; // 플레이어 위치에서 시작하도록 z값 조정
            
            vertices[i + 1] = new Vector3(x, 0, z);
        }
        
        // 삼각형 인덱스는 동일
        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }
        
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        
        return mesh;
    }
    
    // 범위 외곽선 효과
    private IEnumerator CreateRangeOutline(Vector3 center, Vector3 direction, float range, float angle)
    {
        float halfAngle = angle * 0.5f * Mathf.Deg2Rad;
        int segments = 10;
        
        // 왼쪽 경계 방향
        Vector3 leftDir = Quaternion.Euler(0, -angle * 0.5f, 0) * direction;
        // 오른쪽 경계 방향
        Vector3 rightDir = Quaternion.Euler(0, angle * 0.5f, 0) * direction;
        
        // 외곽선 효과 생성
        for (int i = 0; i < 5; i++) // 깜빡임 효과
        {
            // 경계선 파티클 생성
            CreateLineEffect(center, center + leftDir * range, Color.red);
            CreateLineEffect(center, center + rightDir * range, Color.red);
            
            // 호 부분 파티클
            Vector3 prevPoint = center + leftDir * range;
            for (int j = 1; j <= segments; j++)
            {
                float segmentAngle = -halfAngle + ((float)j / segments) * 2 * halfAngle;
                Vector3 dir = Quaternion.Euler(0, segmentAngle * Mathf.Rad2Deg, 0) * direction;
                Vector3 point = center + dir * range;
                
                CreateLineEffect(prevPoint, point, Color.red);
                prevPoint = point;
            }
            
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    // 라인 이펙트 생성
    private void CreateLineEffect(Vector3 start, Vector3 end, Color color)
    {
        int segments = 10;
        for (int i = 0; i < segments; i++)
        {
            float t = (float)i / segments;
            Vector3 pos = Vector3.Lerp(start, end, t);
            pos.y = 0.1f; // 바닥 위에 표시
            
            GameObject dot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            dot.transform.position = pos;
            dot.transform.localScale = Vector3.one * 0.15f;
            
            // 콜라이더 제거
            Destroy(dot.GetComponent<Collider>());
            
            // 파티클 효과 같은 재질
            Material material = new Material(Shader.Find("Standard"));
            material.color = color;
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", color * 2f);
            dot.GetComponent<Renderer>().material = material;
            
            // 잠시 후 사라짐
            Destroy(dot, 0.2f);
        }
    }

    private void CastRangedSkill(Skill skill, Vector3 direction)
    {
        
        // 원거리 스킬 로직 개선
        if (skill.projectilePrefabs != null)
        {
            
            // 방향 벡터 검증 - 제로 벡터인 경우 처리
            if (direction == Vector3.zero)
            {
                // 플레이어가 바라보는 방향을 기본값으로 사용
                direction = player.transform.forward;
                
                // 여전히 제로 벡터라면 기본 방향(앞쪽)으로 설정
                if (direction == Vector3.zero)
                {
                    direction = Vector3.forward;
                }
                
            }
            
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
                case CastPattern.Single:
                    Debug.Log("단일 발사 패턴 실행");
                    FireSingleProjectile(skill, spawnPosition, direction);
                    break;
                    
                case CastPattern.Burst:
                    Debug.Log($"연속 발사 패턴 실행: {skill.burstCount}발, 딜레이={skill.burstDelay}초");
                    StartCoroutine(BurstFire(skill, spawnPosition, direction, skill.burstCount, skill.burstDelay));
                    break;
                    
                case CastPattern.Spread:
                    Debug.Log($"부채꼴 발사 패턴 실행: {skill.spreadCount}발, 각도={skill.spreadAngle}도");
                    SpreadFire(skill, spawnPosition, direction);
                    break;
                    
                case CastPattern.Rain:
                    Debug.Log($"비 발사 패턴 실행: {skill.rainCount}발, 반경={skill.rainRadius}");
                    Vector3 targetPosition = player.transform.position + direction * skill.attackRange;
                    StartCoroutine(ProjectileRain(skill, targetPosition, skill.rainCount));
                    break;
                    
                case CastPattern.Circle:
                    Debug.Log($"원형 발사 패턴 실행: {skill.circleCount}발");
                    CircleFire(skill, spawnPosition);
                    break;
                    
                default:
                    Debug.LogWarning($"정의되지 않은 발사 패턴: {skill.castPattern}, 단일 발사로 대체합니다.");
                    FireSingleProjectile(skill, spawnPosition, direction);
                    break;
            }
            
            // 발사 효과음 재생 (필요시)
            if (skill.soundEffectPrefab != null)
            {
                AudioSource.PlayClipAtPoint(skill.soundEffectPrefab, spawnPosition);
            }
            else
            {
                Debug.LogWarning($"스킬 '{skill._name}'의 효과음이 설정되지 않았습니다.");
            }
            
            // 발사 이펙트 생성 (필요시)
            if (skill.effectPrefab != null)
            {
                GameObject effect = Instantiate(skill.effectPrefab, spawnPosition, Quaternion.LookRotation(direction));
                Destroy(effect, 2f);
            }
            else
            {
                Debug.LogWarning($"스킬 '{skill._name}'의 이펙트가 설정되지 않았습니다.");
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
        //Debug.Log($"CastBuffSkill 함수 진입: 스킬={skill.name}, 버프타입={skill.buffType}");
        
        //// 버프 스킬 로직
        //if (skill.buffType == BuffType.None)
        //{
        //    Debug.LogError($"스킬 '{skill._name}'의 버프 타입이 설정되지 않았습니다.");
        //    return;
        //}
        
        //Debug.Log($"버프 스킬 적용 시작: 유형={skill.buffType}, 값={skill.value}, 지속시간={skill.buffDuration}초");
        
        //// 버프 정보 저장
        //BuffInfo buffInfo = new BuffInfo
        //{
        //    name = skill.name,
        //    type = skill.buffType,
        //    value = skill.value,
        //    duration = skill.buffDuration,
        //    remainingTime = skill.buffDuration
        //};
        
        //// 동일한 버프가 있는지 확인하고 제거
        //activeBuffs.RemoveAll(b => b.type == skill.buffType);
        //Debug.Log($"동일한 유형의 기존 버프 제거: {skill.buffType}");
        
        //// 새 버프 추가
        //activeBuffs.Add(buffInfo);
        //Debug.Log($"새 버프 '{skill.name}' 추가됨, 현재 활성 버프 수: {activeBuffs.Count}");
        
        //// 버프 효과 적용
        //ApplyBuffEffect(buffInfo);
        
        //// 버프 이펙트 생성 (있는 경우)
        //if (skill.effectPrefab != null)
        //{
        //    Debug.Log($"버프 이펙트 생성: {skill.effectPrefab.name}");
        //    GameObject effect = Instantiate(skill.effectPrefab, player.transform.position, Quaternion.identity);
            
        //    // 플레이어 이동에 따라 이펙트도 같이 이동하도록 설정
        //    effect.transform.SetParent(player.transform);
            
        //    // 버프 이펙트 추적을 위해 저장
        //    if (buffEffects == null)
        //        buffEffects = new Dictionary<BuffType, GameObject>();
                
        //    // 기존 이펙트가 있다면 제거
        //    if (buffEffects.ContainsKey(skill.buffType))
        //    {
        //        Debug.Log($"기존 버프 이펙트 제거: {skill.buffType}");
        //        Destroy(buffEffects[skill.buffType]);
        //        buffEffects.Remove(skill.buffType);
        //    }
            
        //    // 새 이펙트 등록
        //    buffEffects.Add(skill.buffType, effect);
            
        //    // 버프 지속시간이 끝나면 이펙트 제거
        //    StartCoroutine(DestroyEffectAfterDuration(skill.buffType, skill.buffDuration));
        //    Debug.Log($"버프 이펙트 '{skill.name}' 등록 완료, {skill.buffDuration}초 후 제거 예정");
        //}
        //else
        //{
        //    Debug.LogWarning($"스킬 '{skill._name}'의 이펙트가 설정되지 않았습니다.");
        //}
        
        //// 버프 사운드 재생 (있는 경우)
        //if (skill.soundEffectPrefab != null)
        //{
        //    Debug.Log($"버프 사운드 재생: {skill.soundEffectPrefab.name}");
        //    AudioSource.PlayClipAtPoint(skill.soundEffectPrefab, player.transform.position);
        //}
        //else
        //{
        //    Debug.LogWarning($"스킬 '{skill._name}'의 효과음이 설정되지 않았습니다.");
        //}
        
        //// 버프 UI 업데이트
        //UpdateBuffUI();
        //Debug.Log("버프 UI 업데이트 완료");
        
        //Debug.Log("CastBuffSkill 함수 종료");
    }
    
    //private IEnumerator DestroyEffectAfterDuration(BuffType buffType, float duration)
    //{
    //    //Debug.Log($"버프 이펙트 제거 코루틴 시작: 유형={buffType}, 지속시간={duration}초");
    //    //yield return new WaitForSeconds(duration);
        
    //    //if (buffEffects != null && buffEffects.ContainsKey(buffType))
    //    //{
    //    //    Debug.Log($"버프 지속시간 종료: 유형={buffType}, 이펙트 제거");
    //    //    Destroy(buffEffects[buffType]);
    //    //    buffEffects.Remove(buffType);
    //    //}
    //}
    
    //private void ApplyBuffEffect(BuffInfo buff)
    //{
    //    Debug.Log($"버프 효과 적용: 이름={buff.name}, 유형={buff.type}, 값={buff.value}");
        
    //    // PlayerStat 컴포넌트 확인
    //    PlayerStat playerStat = player.GetComponent<PlayerStat>();
    //    if (playerStat == null)
    //    {
    //        Debug.LogError("플레이어에서 PlayerStat 컴포넌트를 찾을 수 없습니다.");
    //        return;
    //    }
        
    //    // 버프 타입에 따른 효과 적용
    //    switch (buff.type)
    //    {
    //        case BuffType.Health:
    //            float currentHealth = playerStat.GetStatValue(PlayerStatType.HP);
    //            float maxHealth = playerStat.GetMaxStatValue(PlayerStatType.HP);
    //            float newHealth = Mathf.Min(currentHealth + buff.value, maxHealth);
    //            playerStat.SetStatValue(PlayerStatType.HP, newHealth);
    //            Debug.Log($"체력 회복 적용: {buff.value} 회복, 현재 체력: {newHealth}/{maxHealth}");
    //            break;
                
    //        case BuffType.AttackPower:
    //            float baseAttack = playerStat.GetBaseStatValue(PlayerStatType.Attack);
    //            playerStat.AddStatModifier(PlayerStatType.Attack, buff.value, StatModType.Flat, buff.name);
    //            Debug.Log($"공격력 버프 적용: +{buff.value}, 기본: {baseAttack}, 버프 후: {playerStat.GetStatValue(PlayerStatType.Attack)}");
    //            break;
                
    //        case BuffType.Defense:
    //            float baseDefense = playerStat.GetBaseStatValue(PlayerStatType.Defense);
    //            playerStat.AddStatModifier(PlayerStatType.Defense, buff.value, StatModType.Flat, buff.name);
    //            Debug.Log($"방어력 버프 적용: +{buff.value}, 기본: {baseDefense}, 버프 후: {playerStat.GetStatValue(PlayerStatType.Defense)}");
    //            break;
                
    //        case BuffType.Speed:
    //            float baseSpeed = playerStat.GetBaseStatValue(PlayerStatType.MoveSpeed);
    //            playerStat.AddStatModifier(PlayerStatType.MoveSpeed, buff.value, StatModType.Percent, buff.name);
    //            Debug.Log($"이동속도 버프 적용: +{buff.value}%, 기본: {baseSpeed}, 버프 후: {playerStat.GetStatValue(PlayerStatType.MoveSpeed)}");
    //            break;
                
    //        case BuffType.CriticalRate:
    //            float baseCritRate = playerStat.GetBaseStatValue(PlayerStatType.CriticalRate);
    //            playerStat.AddStatModifier(PlayerStatType.CriticalRate, buff.value, StatModType.Flat, buff.name);
    //            Debug.Log($"치명타율 버프 적용: +{buff.value}, 기본: {baseCritRate}, 버프 후: {playerStat.GetStatValue(PlayerStatType.CriticalRate)}");
    //            break;
                
    //        default:
    //            Debug.LogWarning($"정의되지 않은 버프 타입: {buff.type}");
    //            break;
    //    }
    //}

    // 히트 이펙트 표시
    private void ShowHitEffect(Vector3 position)
    {
        // 간단한 히트 이펙트 (필요에 따라 구현)
        GameObject hitEffect = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        hitEffect.transform.position = position;
        hitEffect.transform.localScale = Vector3.one * 0.5f; // 더 크게 설정
        
        // 콜라이더 제거 (충돌 방지)
        Destroy(hitEffect.GetComponent<Collider>());
        
        // 재질 설정 - 더 눈에 띄는 색상으로
        Material material = new Material(Shader.Find("Standard"));
        material.color = new Color(1f, 0.1f, 0.1f, 0.8f); // 진한 빨간색
        material.EnableKeyword("_EMISSION");
        material.SetColor("_EmissionColor", Color.red * 3f); // 더 강한 발광
        hitEffect.GetComponent<Renderer>().material = material;
        
        // 크기 변화 효과 추가
        StartCoroutine(PulseEffect(hitEffect));
        
        // 짧은 시간 후 제거
        Destroy(hitEffect, 0.4f); // 좀 더 오래 지속
    }
    
    // 펄스 효과 (크기가 커졌다 작아졌다 하는 효과)
    private IEnumerator PulseEffect(GameObject obj)
    {
        float duration = 0.4f;
        float elapsed = 0f;
        Vector3 originalScale = obj.transform.localScale;
        Vector3 targetScale = originalScale * 1.5f;
        
        while (elapsed < duration && obj != null)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // 사인 파형으로 크기 변화 (0->1->0)
            float scale = 1f + 0.5f * Mathf.Sin(t * Mathf.PI);
            obj.transform.localScale = originalScale * scale;
            
            yield return null;
        }
    }
    
    // 디버그 용 Gizmos 그리기 (에디터에서만 보임)
    private Vector3 lastMeleeCenter;
    private Vector3 lastMeleeDirection;
    private float lastMeleeRange;
    private float lastMeleeAngle;
    private bool showLastMeleeRange = false;
    
    private void OnDrawGizmos()
    {
        if (showLastMeleeRange)
        {
            // 부채꼴 범위 그리기
            Gizmos.color = new Color(1f, 0f, 0f, 0.3f); // 빨간색 반투명
            
            // 부채꼴의 중심 표시
            Gizmos.DrawSphere(lastMeleeCenter, 0.2f);
            
            // 부채꼴의 방향 표시
            Gizmos.DrawRay(lastMeleeCenter, lastMeleeDirection * lastMeleeRange);
            
            // 부채꼴 그리기
            float halfAngle = lastMeleeAngle * 0.5f * Mathf.Deg2Rad;
            int segments = 20;
            Vector3 prevPoint = lastMeleeCenter;
            
            // 왼쪽 경계선
            Vector3 leftDir = Quaternion.Euler(0, -lastMeleeAngle * 0.5f, 0) * lastMeleeDirection;
            Gizmos.DrawRay(lastMeleeCenter, leftDir * lastMeleeRange);
            
            // 오른쪽 경계선
            Vector3 rightDir = Quaternion.Euler(0, lastMeleeAngle * 0.5f, 0) * lastMeleeDirection;
            Gizmos.DrawRay(lastMeleeCenter, rightDir * lastMeleeRange);
            
            // 호 그리기
            for (int i = 0; i <= segments; i++)
            {
                float segmentAngle = -halfAngle + ((float)i / segments) * 2 * halfAngle;
                Vector3 dir = Quaternion.Euler(0, segmentAngle * Mathf.Rad2Deg, 0) * lastMeleeDirection;
                Vector3 point = lastMeleeCenter + dir * lastMeleeRange;
                
                if (i > 0)
                {
                    Gizmos.DrawLine(prevPoint, point);
                }
                prevPoint = point;
            }
        }
    }
    
    private IEnumerator HideGizmosAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        showLastMeleeRange = false;
    }
}

