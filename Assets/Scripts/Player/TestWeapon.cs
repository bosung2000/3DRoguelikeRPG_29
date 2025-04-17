using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TestWeapon : MonoBehaviour
{
    private float lastHitTime = -100f;
    public float hitCooldown = 1f;
    public int damage = 10;

    private void OnCollisionEnter(Collision collision)
    {
        if (Time.time - lastHitTime < hitCooldown) return;

        
            TestEnemy testEnemy = collision.gameObject.GetComponent<TestEnemy>();
            if (testEnemy != null)
            {
                testEnemy.TakeDamage(10);
                lastHitTime = Time.time;
            }
    }
}
