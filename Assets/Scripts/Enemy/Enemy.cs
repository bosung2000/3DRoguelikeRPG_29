using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("드랍템 설정")]
    [SerializeField] private GameObject _goldPrefab; //골드 프리펩
    [SerializeField] private GameObject _soulPrefab; //영혼 프리펩

    [Header("투사체 설정")]
    [SerializeField] private Transform _firePoint;
    [SerializeField] private GameObject _projectilePrefab;

    [Header("무기 설정")]
    [SerializeField] private Collider _weaponCollider; //무기 trigger작동

    private RoomZone _parentRoomZone; // 부모 RoomZone 참조

    public EnemyStat Stat { get; private set; }
    public Transform PlayerTarget { get; private set; }
    public GameObject ProjectilePrefab => _projectilePrefab;
    public Transform FirePoint => _firePoint;
    public EnemyRoleType Role => Stat?.StatData.EnemyRole ?? EnemyRoleType.Melee;
    public EnemyType Type => Stat?.StatData.EnemyType ?? EnemyType.Normal;
    public bool IsBoss => Stat?.StatData.EnemyType == EnemyType.Boss;
    public EnemySkillType skillA => Stat?.StatData.SkillA ?? EnemySkillType.None;
    public EnemySkillType skillB => Stat?.StatData.SkillB ?? EnemySkillType.None;
    public int CurrentSkillChoice { get; set; } = 0;
    public int CurrentPhase { get; private set; } = 1; //페이즈 전환
    private float lastSkillTime;
    private EnemyController enemyController;
    private bool _isDeadAnimationEnd = false;
    private bool _isDead = false;
    private Vector3 _cachedTargetPosition;
    private LayerMask layerMask;

    [SerializeField] private GameObject[] _bloodEffect;
    [SerializeField] private Transform _bloodSpawnPoint;
    public Transform BloodSpawnPoint => _bloodSpawnPoint;
    private List<GameObject> activeEffects = new List<GameObject>();

    public event Action<Enemy> OnDeath; //이벤트

    private void Awake()
    {
        Stat = GetComponent<EnemyStat>();
        enemyController = GetComponent<EnemyController>();
        CachePlayer();
        
        // 부모 RoomZone 찾기
        _parentRoomZone = GetComponentInParent<RoomZone>();
    }

    /// 플레이어 찾기
    private void CachePlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerTarget = player.transform;
        }
    }

    private void Start()
    {
        if(_weaponCollider != null)
            _weaponCollider.enabled = false;
    }

    /// 플레이어를 찾지 못하면 다시 시도
    public Transform GetPlayerTarget()
    {
        if (PlayerTarget == null)
        {
            Debug.Log("플레이어를 찾지 못함");
            CachePlayer();
        }
        return PlayerTarget;
    }
    //데미지를 받음
    public void TakeDamage(int damage,bool CriBool)
    {
        if (_isDead) return;

        Stat.ModifyStat(EnemyStatType.HP, -Mathf.Abs(damage));

        // 데미지 텍스트 생성
        ShowDamageText(damage, CriBool);
        DieEffect(0, 0, BloodSpawnPoint, BloodSpawnPoint, 4);
        Debug.Log($" {gameObject.name} {damage} 피해를 입음, 현재 체력: {Stat.GetStatValue(EnemyStatType.HP)}");

        if (Stat.GetStatValue(EnemyStatType.HP) <= 0)
        {
            Debug.Log("데미지를 받아 죽음");
            Die();
        }
        else
        {
            if (enemyController != null)
            {
                enemyController.ResetAttackCooldown();
                enemyController.ChageState(EnemyStateType.Hit);
            }
            else
            {
                Debug.Log("컨트롤러가 널임");
                return;
            }
        }

        // 보스만 페이즈 전환 체크
        if (IsBoss && CurrentPhase == 1)
        {
            float hpRatio = Stat.GetStatValue(EnemyStatType.HP) / Stat.GetStatValue(EnemyStatType.MaxHP);
            if (hpRatio <= 0.5f)
            {
                CurrentPhase = 2;
                Debug.Log("보스 2페이즈 진입!");
                // 2페이즈 진입 연출 필요시 여기에 trigger
            }
        }
    }

    // 데미지 텍스트 표시 메서드
    private void ShowDamageText(int damage, bool isCritical)
    {
        // DamageTextManager를 통해 데미지 텍스트 표시

        if (DamageTextManager.Instance != null)
        {
            DamageTextManager.Instance.ShowDamageText(transform.position, damage, isCritical);
            return;
        }
    }

    //죽음 - 재화 드랍
    public void Die()
    {
        if (_isDead) return;
        _isDead = true;

        if (IsBoss ==true)
        {
            GameManager.Instance.MapManager.OnBossDefeated();
            GameManager.Instance.PortalManager._unlockedPortals.Clear();
        }

        if (enemyController != null)
        {
            enemyController.animator.SetTrigger("Die");
            enemyController.ChageState(EnemyStateType.Dead);
        }

        DropCurrency();

        //존 내의 적 처치 이벤트
        OnDeath?.Invoke(this);
        
    }
    
    //재화 드랍
    private void DropCurrency()
    {
        SpawnCurrency(_goldPrefab, (int)Stat.GetStatValue(EnemyStatType.Gold));
        SpawnCurrency(_soulPrefab, (int)Stat.GetStatValue(EnemyStatType.Soul));
    }

    //재화 생성 관련
    private void SpawnCurrency(GameObject prefab, int amount)
    {
        if (prefab == null || amount <= 0) return;

        Vector3 dropPos = GetSafeDropPosition(transform.position, 1.5f);
        GameObject Obj = Instantiate(prefab, dropPos, Quaternion.identity, _parentRoomZone?.transform);

        CurrencyData currencyData = Obj.GetComponent<CurrencyData>();
        if (currencyData != null)
        {
            currencyData.SetAmount(amount);
        }
    }
    //생성 위치 보정
    private Vector3 GetSafeDropPosition(Vector3 center, float radius = 1.5f)
    {
        Vector2 circle = UnityEngine.Random.insideUnitCircle * radius;
        Vector3 randomXZ = new Vector3(circle.x, 0f, circle.y);
        Vector3 dropPos = center + randomXZ;

        if(NavMesh.SamplePosition(dropPos, out NavMeshHit hit, radius, NavMesh.AllAreas))
        {
            return hit.position + Vector3.up * 0.3f;
        }

        return center + Vector3.up * 0.5f;
    }
    //Die 애니메이션 끝남을 확임
    public void OnDeadAnimationEnd()
    {
        //죽는 애니메이션이 끝남을 표시
        _isDeadAnimationEnd = true;
    }
    public bool IsDeadAnimationEnded()
    {
        return _isDeadAnimationEnd;
    }

    public bool CanEnterSkillState()
    {
        return Type != EnemyType.Normal && CanUseSkill();
    }

    //공격 - 콜라이더
    //ON
    public void EnableWeaponCollider()
    {
        if (!(enemyController._currentState is EnemyAttackState) || _weaponCollider == null) return;
        
        _weaponCollider.enabled = true;
    }
    //OFF
    public void DisableWeaponCollider()
    {
        if (!(enemyController._currentState is EnemyAttackState) || _weaponCollider == null) return;

        _weaponCollider.enabled = false;
    }

    //트리거접촉 시
    private void OnTriggerEnter(Collider other)
    {
        if (_weaponCollider == null || !_weaponCollider.enabled) return;
        if(!other.CompareTag("Player")) return;

        if(other.CompareTag("Player"))
        {
            if (other.TryGetComponent(out PlayerStat playerStat))
            {
                int damage = (int)Stat.GetStatValue(EnemyStatType.Attack);
                playerStat.TakeDamage(damage);
            }
        }

        DisableWeaponCollider();
    }

    //공격 끝나는 시간 체크
    public void OnAttackAnimationEnd()
    {
        if(enemyController != null && enemyController.CurrentStateType == EnemyStateType.Attack)
        {
            enemyController.ChageState(EnemyStateType.Chase);
        }
    }

    //타겟 위치 저장
    public void CachedTargetPosition(Vector3 position)
    {
        position.y += 1.0f;
        _cachedTargetPosition = position;
    }

    //원거리 공격
    public void FireProjectile()
    {
        if (enemyController == null || enemyController.CurrentStateType != EnemyStateType.Attack)
        {
            return;  // 공격 상태가 아니면 발사 안 함
        }

        if (ProjectilePrefab == null || _firePoint == null) return;

        Transform target = GetPlayerTarget();
        if (target == null) return;

        Vector3 spawnPos = FirePoint.position;
        Vector3 dir = (_cachedTargetPosition - spawnPos).normalized;
        Quaternion rot = Quaternion.LookRotation(dir);

        GameObject projectile = GameObject.Instantiate(ProjectilePrefab, spawnPos, rot);
        Projectile proj = projectile.GetComponent<Projectile>();
        if (proj != null)
        {
            int damage = (int)Stat.GetStatValue(EnemyStatType.Attack);
            proj.Intialize(dir, damage);
        }
    }

    //스킬
    public string GetSkillTriggerName(EnemySkillType type)
    {
        return type switch
        {
            EnemySkillType.SpreadShot => "Skill_SpreadShot",
            EnemySkillType.Dash => "Skill_Dash",
            _ => string.Empty
        };
    }
    public EnemySkillType GetCurrentSkillType()
    {
        if (!IsBoss)
            return skillB;

        return CurrentPhase == 1 ? skillA : (CurrentSkillChoice == 0 ? skillA : skillB);
    }

    //스킬별 범위
    public float GetSkillRange()
    {
        if (IsBoss)
        {
            var skillType = GetCurrentSkillType();
            return skillType switch
            {
                EnemySkillType.Dash => 8f,
                _ => 0f
            };
        }
        else
        {
            return skillB switch
            {
                EnemySkillType.Dash => 10f,
                EnemySkillType.SpreadShot => 7f,
                _ => 0f
            };
        }
    }
    //쿨타임관리
    public bool CanUseSkill()
    {
        float cooldown = Stat?.StatData.SkillCooldown ?? 8f;

        // 보스
        if (IsBoss)
        {
            return (skillA != EnemySkillType.None || skillB != EnemySkillType.None)
                && Time.time >= lastSkillTime + cooldown;
        }

        // 엘리트
        return skillB != EnemySkillType.None && Time.time >= lastSkillTime + cooldown;
    }

    public void ResetSkillCooldown()
    {
        lastSkillTime = Time.time;
    }

    //근거리
    //dash
    public void SkillDash()
    {
        StartCoroutine(DashCoroutine());
    }

    private IEnumerator DashCoroutine()
    {
        float dashTime = 0.5f;
        float dashSpeed = 10.5f;
        float hitRadius = 1.0f;

        Vector3 dir = (GetPlayerTarget().position - transform.position ).normalized;
        dir.y = 0f;

        Quaternion initialRotation = Quaternion.LookRotation(dir);
        transform.rotation = initialRotation;

        float timer = 0f;
        //agent 비활성
        enemyController.agent.enabled = false;
        enemyController.agent.updateRotation = false;

        while (timer < dashTime)
        {
            transform.position += dir * dashSpeed * Time.deltaTime;
            Collider[] hits = Physics.OverlapSphere(transform.position, hitRadius, LayerMask.GetMask("Player"));
            foreach(var hit in hits)
            {
                if(hit.CompareTag("Player"))
                {
                    PlayerStat player = hit.GetComponent<PlayerStat>();
                    if(player != null )
                    {
                        player.TakeDamage(20);
                    }

                    timer = dashTime;
                    break;
                }
            }

            timer += Time.deltaTime;
            yield return null;
        }

        //돌진 후 회전
        Vector3 toTarget = GetPlayerTarget().position - transform.position;
        toTarget.y = 0;
        if (toTarget != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(toTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 1f); // 즉시 회전
        }

        enemyController.agent.enabled = true;
        enemyController.agent.updateRotation = true;
        enemyController.agent.Warp(transform.position);
    }
    //충격파
    public void SkillJumpStomp()
    {
        //StartCoroutine(JumpStompCoroutine());
        float stompRadius = 4f;
        Vector3 stompCenter = transform.position;

        Collider[] hits = Physics.OverlapSphere(stompCenter, stompRadius, LayerMask.GetMask("Player"));
        foreach(var hit in hits)
        {
            if (hit.TryGetComponent(out PlayerStat player))
            {
                int damage = (int)Stat.GetStatValue(EnemyStatType.Attack);
                player.TakeDamage(damage);
            }
        }
    }



    //원거리
    //SpreadShot
    public void SkillSpreadShot()
    {
        if (ProjectilePrefab == null || FirePoint == null) return;

        int count = 5; // 발사 갯수
        float angleSpread = 30f; //퍼지는 각도
        float angleStep = angleSpread / (count - 1);
        float startAngle = -angleSpread / 2f;

        Vector3 spawnBase = FirePoint.position;
        Vector3 baseDir = (GetPlayerTarget().position - spawnBase).normalized;
        Quaternion baseRot = Quaternion.LookRotation(baseDir);

        for (int i = 0; i < count; i++)
        {
            float angle = startAngle + i * angleStep;
            Quaternion spreadRot = Quaternion.Euler(0, angle, 0) * baseRot;
            Vector3 dir = spreadRot * Vector3.forward;

            Vector3 spawnPos = FirePoint.position + Vector3.up * 1.2f + dir * 1.0f;

            GameObject projectile = Instantiate(ProjectilePrefab, spawnPos, Quaternion.LookRotation(dir));
            if (projectile.TryGetComponent(out Projectile proj))
            {
                int damage = (int)Stat.GetStatValue(EnemyStatType.Attack);
                proj.Intialize(dir, damage);
            }
        }
    }
    public void DieEffect(float delay, int index, Transform position, Transform rotation, float duration)
    {
        CleanupActiveEffects();
        StartCoroutine(SpawnEffectAfterSeconds(delay, index, position, rotation, duration));
    }
    private IEnumerator SpawnEffectAfterSeconds(float delay, int index, Transform position, Transform rotation, float duration)
    {
        float elapsed = 0f;
        while (elapsed < delay)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        GameObject effect = Instantiate(_bloodEffect[index], position.position, rotation.rotation);
        activeEffects.Add(effect);

        StartCoroutine(DestroyEffectAfterSeconds(effect, duration));
    }

    private IEnumerator DestroyEffectAfterSeconds(GameObject obj, float delay)
    {
        float elapsed = 0f;
        while (elapsed < delay)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        if (obj != null) Destroy(obj);
    }

    private void CleanupActiveEffects()
    {
        foreach (GameObject effect in activeEffects)
        {
            if (effect != null)
            {
                Destroy(effect);
            }
        }
        activeEffects.Clear();
    }










    //임시 몬스터 제거 키 추후 삭제
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("적 죽음");
            Die();
        }
    }
}