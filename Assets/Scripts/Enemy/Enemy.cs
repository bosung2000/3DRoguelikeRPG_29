using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{
    public EnemyStat Stat { get; private set; }
    public Transform PlayerTarget {  get; private set; }
    public EnemyRoleType Role => Stat?.StatData.EnemyRole ?? EnemyRoleType.Melee;

    [Header("드랍템 설정")]
    [SerializeField] private GameObject _goldPrefab; //골드 프리펩
    [SerializeField] private GameObject _soulPrefab; //영혼 프리펩

    private EnemyController enemyController;
    private bool _isDeadAnimationEnd = false;

    private void Awake()
    {
        Stat = GetComponent<EnemyStat>();
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
        Stat.ModifyStat(EnemyStatType.HP, -Mathf.Abs(damage));
        
        
        //if(Stat.GetStatValue(EnemyStatType.HP) <= 0)
        //{
        //    Die();
        //}
        //else
        //{
        //    enemyController.ChageState(EnemyStateType.Hit);
        //}

    }
    public void Die()
    {
        Debug.Log($"처치");

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
        int dropSoul = (int)Stat.GetStatValue(EnemyStatType.Soul);

        for (int i = 0; i < dropGold; i++)
        {
            Vector3 dropGPos = transform.position + new Vector3(Random.Range(-1f, 1f), 0.5f, Random.Range(-1f, 1f));
            Instantiate(_goldPrefab, dropGPos, Quaternion.identity);
        }

        for (int i = 0; i < dropSoul; i++)
        {
            Vector3 dropSPos = transform.position + new Vector3(0f, 0.5f, 0f);
            Instantiate(_soulPrefab, dropSPos, Quaternion.identity);
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
