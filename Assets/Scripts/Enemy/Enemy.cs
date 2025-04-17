using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{
    public EnemyController enemyController {  get; private set; }
    public EnemyStat Stat { get; private set; }
    public Transform PlayerTarget {  get; private set; }
    public EnemyRoleType Role => Stat?.StatData.EnemyRole ?? EnemyRoleType.Melee;

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
        Stat.ModifyStat(EnemyStatType.HP, -Mathf.Abs(damage));

        Debug.Log($" {gameObject.name} {damage} 피해를 입음, 현재 체력: {Stat.GetStatValue(EnemyStatType.HP)}");

        if (Stat.GetStatValue(EnemyStatType.HP) <= 0)
        {
            Debug.Log("데미지를 받아 죽음");
            Die();
        }
        else
        {
            if(enemyController != null)
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
        int drop = (int)Stat.GetStatValue(EnemyStatType.Gold);
        
        //임시코드
        Debug.Log($"사망, 골드 {drop} 드랍");
    }

    public void FixedUpdate()
    {
        float currentHP = Stat.GetStatValue(EnemyStatType.HP);
        //Debug.Log($"{gameObject.name} 현재 체력: {currentHP}");
    }
}
