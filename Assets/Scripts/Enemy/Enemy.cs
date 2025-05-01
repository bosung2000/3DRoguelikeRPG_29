using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UI;

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

    public EnemyStat Stat { get; private set; }
    public Transform PlayerTarget { get; private set; }
    public GameObject ProjectilePrefab => _projectilePrefab;
    public Transform FirePoint => _firePoint;
    public EnemyRoleType Role => Stat?.StatData.EnemyRole ?? EnemyRoleType.Melee;

    private EnemyController enemyController;
    private bool _isDeadAnimationEnd = false;
    private bool _isDead = false;
    private Vector3 _cachedTargetPosition;

    public event Action<Enemy> OnDeath; //이벤트

    private void Awake()
    {
        Stat = GetComponent<EnemyStat>();
        enemyController = GetComponent<EnemyController>();
        CachePlayer();
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
            }
        }
    }

    // 데미지 텍스트 표시 메서드
    private void ShowDamageText(int damage, bool isCritical)
    {
        // 데미지 텍스트 프리팹 로드
        GameObject damageTextPrefab = Resources.Load<GameObject>("UI/DamageText");
        if (damageTextPrefab == null)
        {
            Debug.LogError("DamageText 프리팹을 찾을 수 없습니다. Resources/UI/DamageText 경로를 확인하세요.");
            return;
        }

        // 몬스터 위 위치 계산
        Vector3 position = transform.position + Vector3.up * 1.5f;
        position += new Vector3(UnityEngine.Random.Range(-0.3f, 0.3f), UnityEngine.Random.Range(0f, 0.3f), 0);
        
        // 새로운 GameObject 생성하여 World Space Canvas 추가
        GameObject canvasObj = new GameObject("DamageTextCanvas");
        canvasObj.transform.position = position;
        
        // 항상 카메라를 바라보도록 설정
        canvasObj.transform.forward = Camera.main.transform.forward;
        
        // Canvas 컴포넌트 추가 및 설정
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        
        // 캔버스 스케일러 추가 및 설정
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 100;
        
        // 레이캐스터 추가 (UI 이벤트를 위해)
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // 데미지 텍스트 생성
        GameObject textObj = Instantiate(damageTextPrefab, Vector3.zero, Quaternion.identity, canvasObj.transform);
        RectTransform rectTransform = textObj.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            // 로컬 위치 초기화 (캔버스 내에서 중앙에 위치)
            rectTransform.localPosition = Vector3.zero;
            // 크기 설정
            rectTransform.sizeDelta = new Vector2(200, 50);
            // 회전 초기화 - 중요: 로컬 회전을 초기화하여 텍스트가 정면을 향하도록 함
            rectTransform.localRotation = Quaternion.identity;
        }
        
        // 텍스트 내의 모든 자식 오브젝트도 로컬 회전 초기화
        foreach (Transform child in textObj.transform)
        {
            child.localRotation = Quaternion.identity;
        }
        
        // Canvas 크기 설정
        canvas.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        
        DamageText damageText = textObj.GetComponent<DamageText>();
        if (damageText != null)
        {
            damageText.SetDamageText(damage, isCritical);
        }
        
        // 캔버스와 텍스트 일정 시간 후 제거 (DamageText 스크립트에서 처리하도록 수정)
        Destroy(canvasObj, 2.0f);
    }

    //죽음 - 재화 드랍
    public void Die()
    {
        if (_isDead) return;
        _isDead = true;

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
        GameObject Obj = Instantiate(prefab, dropPos, Quaternion.identity);

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
        
    //공격 - 콜라이더
    //ON
    public void EnableWeaponCollider()
    {
        if(_weaponCollider != null)
            _weaponCollider.enabled = true;
    }
    //OFF
    public void DisableWeaponCollider()
    {
        if (_weaponCollider != null)
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
            enemyController.ChageState(EnemyStateType.KeepDistance);
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
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("적 죽음");
            Die();
        }
    }


}