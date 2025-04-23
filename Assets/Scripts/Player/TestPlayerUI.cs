using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestPlayerUI : MonoBehaviour
{
    [SerializeField] Player _player;
    [SerializeField] PlayerStat _playerStat;
    [SerializeField] CurrencyManager _currencyManager;
    [SerializeField] TextMeshProUGUI _GoldText;
    [SerializeField] TextMeshProUGUI _soulText;

    [SerializeField] Button _DashBtn;
    [SerializeField] TextMeshProUGUI _maxHPText;
    [SerializeField] TextMeshProUGUI _hpText;
    [SerializeField] TextMeshProUGUI _maxMPText;
    [SerializeField] TextMeshProUGUI _mpText;
    [SerializeField] TextMeshProUGUI _speedText;
    [SerializeField] TextMeshProUGUI _attackText;
    [SerializeField] TextMeshProUGUI _dmgReductionText;
    [SerializeField] TextMeshProUGUI _criticalChanceText;
    [SerializeField] TextMeshProUGUI _criticalDamageText;
    [SerializeField] TextMeshProUGUI _DashDistance;
    [SerializeField] TextMeshProUGUI _DashCooldownText;

    private Coroutine _cooldownRoutine;

    private void Start()
    {
        _playerStat.OnStatsChanged += UpdateStats;
        _currencyManager.OnGoldChange += UpdateGold;
        _currencyManager.OnSoulChange += UpdateSoul;
    }
    private void UpdateStats(PlayerStat playerStat)
    {
        //_maxHPText.text = playerStat.GetStatValue(PlayerStatType.MaxHP).ToString("F0");  
        _hpText.text = "Hp : "+playerStat.GetStatValue(PlayerStatType.HP).ToString("F0");
        //_maxMPText.text = playerStat.GetStatValue(PlayerStatType.MaxMP).ToString("F0");  
        //_mpText.text = playerStat.GetStatValue(PlayerStatType.MP).ToString("F0");  
        _speedText.text = "Speed : " + playerStat.GetStatValue(PlayerStatType.MoveSpeed).ToString("F0");
        //_attackText.text = playerStat.GetStatValue(PlayerStatType.Attack).ToString("F0");  
        //_dmgReductionText.text = playerStat.GetStatValue(PlayerStatType.DMGReduction).ToString("F0");  
        //_criticalChanceText.text = playerStat.GetStatValue(PlayerStatType.CriticalChance).ToString("F0");  
        //_criticalDamageText.text = playerStat.GetStatValue(PlayerStatType.CriticalDamage).ToString("F0");
    }

    private void UpdateGold(int gold)
    {
        _GoldText.text = GameManager.Instance.PlayerManager.Currency.currencies[CurrencyType.Gold].ToString("F0");
    }
    private void UpdateSoul(int soul)
    {
        _soulText.text = GameManager.Instance.PlayerManager.Currency.currencies[CurrencyType.Soul].ToString("F0");
    }
    public void StartDashCooldown()
    {
        float dashCooldown = _playerStat.GetStatValue(PlayerStatType.DashCooldown);

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
            Image img = _DashBtn.GetComponent<Image>();
            img.color = new Color(img.color.r, img.color.g, img.color.b, 0.5f);
            _DashCooldownText.text = $"{remaining:F1}";
            remaining -= Time.deltaTime;
            yield return null;
            img.color = new Color(img.color.r, img.color.g, img.color.b, 1f);
        }

        _DashCooldownText.text = "";
    }
}

