using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private Enemy _enemy;

    private int testDamage = 10;
    private KeyCode damageKey = KeyCode.Q;

    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
    }

    private void Start()
    {

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
        _enemy.TakeDamage(damage,false);
    }
}