using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;


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

    public event Action<Enemy> OnDeath; //이벤트



    private void Awake()
    {
        Stat = GetComponent<EnemyStat>();
        enemyController = GetComponent<EnemyController>();
        CachePlayer();
    }

    /// <summary>
    /// 플레이어 찾기
    /// </summary>
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
    /// <summary>
    /// 플레이어를 찾지 못하면 다시 시도
    /// </summary>
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
    public void TakeDamage(int damage)
    {
        if (_isDead) return;

        Stat.ModifyStat(EnemyStatType.HP, -Mathf.Abs(damage));

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
        if (_weaponCollider == null || !_weaponCollider.enabled)
            return;

        if(other.CompareTag("Player"))
        {
            if(other.TryGetComponent(out PlayerStat playerStat))
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

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("적 죽음");
            Die();
        }
    }
}