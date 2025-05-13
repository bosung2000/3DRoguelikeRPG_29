using System.Collections.Generic;
using UnityEngine;

public class FireComboHitBox : MonoBehaviour
{
    private int damage;
    private bool isCritical;
    private HashSet<Enemy> alreadyHit;

    public void Init(int dmg, bool critical, HashSet<Enemy> hitSet)
    {
        damage = dmg;
        isCritical = critical;
        alreadyHit = hitSet;
    }

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && alreadyHit != null && !alreadyHit.Contains(enemy))
        {
            enemy.TakeDamage(damage, isCritical);
            alreadyHit.Add(enemy);
        }
    }
} 