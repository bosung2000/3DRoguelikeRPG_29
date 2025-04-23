using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _speed = 10f; //투사체 속도 설정
    [SerializeField] private int _damage;         //데이터에서 가져오기

    private Vector3 _direction;

    private void FixedUpdate()
    {
        transform.position += _direction * _speed * Time.deltaTime;
    }

    public void Intialize(Vector3 dir, int dmg)
    {
        _direction = dir.normalized;
        _damage = dmg;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            return;
        }

        if(other.CompareTag("Player"))
        {
            PlayerStat player = other.GetComponentInParent<PlayerStat>();
            if(player != null)
            {
                player.TakeDamage(_damage);
                Debug.Log(_damage);
            }
        }
        
        Destroy(gameObject);
        return;
    }
}
