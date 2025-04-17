using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;

public class TestEnemy : MonoBehaviour
{
    [SerializeField] int _maxHP = 100;
    [SerializeField] int _currentHP = 100;
    [SerializeField] TextMeshProUGUI _hpText;

    private float lastHitTime = -100f;
    public float hitCooldown = 1f;
    public int damage = 10;

    public event Action<TestEnemy> OnEnermyStatsChanged;

    private void Awake()
    {
        _currentHP = _maxHP;
        UpdateHPText();
    }

    private void Start()
    {
        OnEnermyStatsChanged += (enemy) => UpdateHPText();
    }

    private void UpdateHPText()
    {
        if (_hpText != null)
        {
            _hpText.text = $"{_currentHP}/{_maxHP}";
        }
    }

    private void OnStatChanged()
    {
        OnEnermyStatsChanged?.Invoke(this);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Time.time - lastHitTime < hitCooldown) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damage);
                lastHitTime = Time.time;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        _currentHP -= Mathf.Max(_currentHP - damage, 0);
        OnStatChanged();

        if (_currentHP <= 0)
        {
            Debug.Log("Enemy is dead");
        }
    }
}
