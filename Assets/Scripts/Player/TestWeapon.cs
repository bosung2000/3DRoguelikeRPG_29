using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TestWeapon : MonoBehaviour
{
    [SerializeField] PlayerStat _playerStat;
    [SerializeField] Collider _weaponCollider;

    private HashSet<Enemy> _hitEnemies = new HashSet<Enemy>();
    private void Awake()
    {
        _playerStat = GetComponentInParent<PlayerStat>();
        _weaponCollider = GetComponent<Collider>();
        _weaponCollider.enabled = false;
    }
    /// <summary>
    /// 공격 콜라이더 활성화
    /// </summary>
    public void EnableCollider()
    {
        _hitEnemies.Clear();
        _weaponCollider.enabled = true;
    }

    /// <summary>
    /// 공격 콜라이더 비활성화
    /// </summary>
    public void DisableCollider()
    {
        _weaponCollider.enabled = false;
    }
    /// <summary>
    /// 적과 충돌 시 공격
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();

        if (other.CompareTag("Enemy")&& !_hitEnemies.Contains(enemy))
        {
            _playerStat.Attack(enemy);
            _hitEnemies.Add(enemy);
        }
    }
}
