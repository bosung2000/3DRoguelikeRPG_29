using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;

public class TestEnemy : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _hpText;
    [SerializeField] TextMeshProUGUI _GoldText;
    [SerializeField] TextMeshProUGUI _soulText;
    [SerializeField] EnemyStat _enemyStat;

    private float lastHitTime = -100f;
    public float hitCooldown = 1f;
    public int damage = 10;

    public event Action<TestEnemy> OnEnermyStatsChanged;


    private void Start()
    {
        OnEnermyStatsChanged += (enemy) => UpdateHPText();
    }
    void Update()
    {
        if (_enemyStat != null)
        {
            UpdateHPText();
        }
    }
    private void UpdateHPText()
    {
        if (_hpText != null)
        {
            int _currentHP = (int)_enemyStat.GetStatValue(EnemyStatType.HP);
            int _maxHP = (int)_enemyStat.GetStatValue(EnemyStatType.MaxHP);
            _hpText.text = $"{_currentHP}/{_maxHP}";
        }
        if (_GoldText != null)
        {
            int _gold = (int)GameManager.Instance.PlayerManager.Currency.currencies[CurrencyType.Gold];
            _GoldText.text = $"{_gold}";
        }
        if (_soulText != null)
        {
            int _soul = (int)GameManager.Instance.PlayerManager.Currency.currencies[CurrencyType.Soul];
            _soulText.text = $"{_soul}";
        }
    }

    public void OnStatChanged()
    {
        OnEnermyStatsChanged?.Invoke(this);
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (Time.time - lastHitTime < hitCooldown) return;

    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        Player player = collision.gameObject.GetComponent<Player>();
    //        if (player != null)
    //        {
    //            player.TakeDamage((int)_enemyStat.GetStatValue(EnemyStatType.Attack));
    //            lastHitTime = Time.time;
    //        }
    //    }
    //}

    //public void TakeDamage(int damage)
    //{
    //    if (Time.time - lastHitTime < hitCooldown) return;
        

    //    _currentHP = Mathf.Max(_currentHP - damage, 0);
    //    OnStatChanged();

    //    lastHitTime = Time.time;
    //    if (_currentHP <= 0)
    //    {
    //        Debug.Log("Enemy is dead");
    //    }
    //}
}
