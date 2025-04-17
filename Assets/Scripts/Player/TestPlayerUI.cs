using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestPlayerUI : MonoBehaviour
{
    [SerializeField] Player _player;
    [SerializeField] PlayerStat _playerStat;

    [SerializeField] TextMeshProUGUI _maxHPText;
    [SerializeField] TextMeshProUGUI _hpText;
    [SerializeField] TextMeshProUGUI _maxMPText;
    [SerializeField] TextMeshProUGUI _mpText;
    [SerializeField] TextMeshProUGUI _speedText;
    [SerializeField] TextMeshProUGUI _attackText;
    [SerializeField] TextMeshProUGUI _dmgReductionText;
    [SerializeField] TextMeshProUGUI _criticalChanceText;
    [SerializeField] TextMeshProUGUI _criticalDamageText;

    [SerializeField] TextMeshProUGUI _cooldownText;
    private Coroutine _cooldownRoutine;

    private void Start()
    {
        _playerStat.OnStatsChanged += UpdateStats;
    }
    private void UpdateStats(PlayerStat playerStat)
    {
        //_maxHPText.text = playerStat.GetStatValue(PlayerStatType.MaxHP).ToString("F0");
        _hpText.text = playerStat.GetStatValue(PlayerStatType.HP).ToString("F0");
        //_maxMPText.text = playerStat.GetStatValue(PlayerStatType.MaxMP).ToString("F0");
        //_mpText.text = playerStat.GetStatValue(PlayerStatType.MP).ToString("F0");
        _speedText.text = playerStat.GetStatValue(PlayerStatType.MoveSpeed).ToString("F0");
        //_attackText.text = playerStat.GetStatValue(PlayerStatType.Attack).ToString("F0");
        //_dmgReductionText.text = playerStat.GetStatValue(PlayerStatType.DMGReduction).ToString("F0");
        //_criticalChanceText.text = playerStat.GetStatValue(PlayerStatType.CriticalChance).ToString("F0");
        //_criticalDamageText.text = playerStat.GetStatValue(PlayerStatType.CriticalDamage).ToString("F0");
        //UpdateCooldownUI();
    }

    public void StartDashCooldown()
    {
        float dashCooldown = _playerStat.GetStatValue(PlayerStatType.DashCooltime);

        if (_cooldownRoutine != null)
        { 
            StopCoroutine(_cooldownRoutine); 
        }

        _cooldownRoutine = StartCoroutine(ShowCooldownRoutine(dashCooldown));
    }
    private IEnumerator ShowCooldownRoutine(float total)
    {
        float remaining = total;

        while (remaining > 0)
        {
            _cooldownText.text = $"{remaining:F1}";
            remaining -= Time.deltaTime;
            yield return null;
        }

        _cooldownText.text = "Dash";
    }
}

