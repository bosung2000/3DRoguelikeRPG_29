using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TestWeapon : MonoBehaviour
{
    private float lastHitTime = -100f;
    public float hitCooldown = 1f;
    [SerializeField] PlayerStat _playerStat;

    private void OnTriggerEnter(Collider other)
    {
        if (Time.time - lastHitTime < hitCooldown) return;

            //TestEnemy testEnemy = other.GetComponent<TestEnemy>();
            Enemy enemy = other.GetComponent<Enemy>();

        if (other.CompareTag("Enemy"))
        {
            enemy.TakeDamage((int)_playerStat.GetStatValue(PlayerStatType.Attack));
            lastHitTime = Time.time;
        }
    }
}