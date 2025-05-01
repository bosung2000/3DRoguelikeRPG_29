using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private Enemy _enemy;

    private int testDamage = 10;
    private KeyCode damageKey = KeyCode.Q;
    private Player player;

    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
    }

    private void Start()
    {
        player =FindObjectOfType<Player>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(damageKey))
        {
            ReceiveDamage(testDamage);
        }
    }

    public void ReceiveDamage(int damage)
    {
        if (_enemy == null) return;
        _enemy.TakeDamage(damage,false,player._playerStat);
    }
}