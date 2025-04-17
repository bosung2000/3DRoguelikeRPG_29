using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TestWeapon : MonoBehaviour
{
    private float lastHitTime = -100f;
    public float hitCooldown = 1f;
    public int damage = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (Time.time - lastHitTime < hitCooldown) return;

            TestEnemy testEnemy = other.GetComponent<TestEnemy>();

        if (other.CompareTag("Enemy"))
        {
            testEnemy.TakeDamage(damage);
            lastHitTime = Time.time;
        }
    }
}
