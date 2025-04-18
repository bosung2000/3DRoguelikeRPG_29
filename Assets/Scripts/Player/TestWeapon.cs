using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TestWeapon : MonoBehaviour
{
    [SerializeField] PlayerStat _playerStat;

    private void Awake()
    {
        _playerStat = GetComponentInParent<PlayerStat>();
    }
    private void OnTriggerEnter(Collider other)
    {
        //TestEnemy testEnemy = other.GetComponent<TestEnemy>();
        Enemy enemy = other.GetComponent<Enemy>();

        if (other.CompareTag("Enemy"))
        {
            enemy.TakeDamage((int)_playerStat.GetStatValue(PlayerStatType.Attack));
        }
    }
}
