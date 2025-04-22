using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{
    public EnemyStat Stat { get; private set; }
    public Transform PlayerTarget { get; private set; }
    public EnemyRoleType Role => Stat?.StatData.EnemyRole ?? EnemyRoleType.Melee;

    private bool _isDead = false;


    [Header("드랍템 설정")]
    [SerializeField] private GameObject _goldPrefab; //골드 프리펩
    [SerializeField] private GameObject _soulPrefab; //영혼 프리펩

    private EnemyController enemyController;
    private bool _isDeadAnimationEnd = false;


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
                Debug.Log("아직 체력이 남았다");
                enemyController.ResetAttackCooldown();
                enemyController.ChageState(EnemyStateType.Hit);
            }
            else
            {
                Debug.Log("컨트롤러가 널임");
            }
        }

    }
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
    }

    private void DropCurrency()
    {
        int dropGold = (int)Stat.GetStatValue(EnemyStatType.Gold);
        int dropSoul =  (int)Stat.GetStatValue(EnemyStatType.Soul);

        if(dropGold > 0 && _goldPrefab != null)
        {
            Vector3 dropPos = transform.position + new Vector3(Random.Range(-1f, 1f), 0.5f, Random.Range(-1f, 1f));
            Instantiate(_goldPrefab, dropPos, Quaternion.identity);

            CurrencyData currencyData = _goldPrefab.GetComponent<CurrencyData>();
            if(currencyData != null)
            {
                currencyData.SetAmount(dropGold);
            }
        }

        if (dropSoul > 0 && _soulPrefab != null)
        {
            Vector3 dropPos = transform.position + new Vector3(Random.Range(-1f, 1f), 0.5f, Random.Range(-1f, 1f));
            Instantiate(_soulPrefab, dropPos, Quaternion.identity);

            CurrencyData currencyData = _soulPrefab.GetComponent<CurrencyData>();
            if (currencyData != null)
            {
                currencyData.SetAmount(dropSoul);
            }
        }
    }

    public void OnDeadAnimationEnd()
    {
        //죽는 애니메이션이 끝남을 표시
        _isDeadAnimationEnd = true;
    }

    public bool IsDeadAnimationEnded()
    {
        return _isDeadAnimationEnd;
    }
}