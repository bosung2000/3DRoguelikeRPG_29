using System.Collections.Generic;
using UnityEngine;

public class LightingSlashProjectile : MonoBehaviour
{
    public float speed;
    public float lifeTime;
    public int damage;
    public HashSet<Enemy> alreadyHitEnemies;

    private Vector3 moveDir;

    public void Init(Vector3 direction, int dmg, float spd, float life, HashSet<Enemy> hitSet)
    {
        moveDir = direction.normalized;
        damage = dmg;
        speed = spd;
        lifeTime = life;
        alreadyHitEnemies = hitSet;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += moveDir * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && alreadyHitEnemies != null && !alreadyHitEnemies.Contains(enemy))
        {
            enemy.TakeDamage(damage, true);
            alreadyHitEnemies.Add(enemy);
        }
    }
} 