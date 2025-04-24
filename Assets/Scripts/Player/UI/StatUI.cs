using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatUI : MonoBehaviour
{
    [SerializeField] PlayerStat _player;

    [SerializeField] Button _dashBtn;
    [SerializeField] TextMeshProUGUI _hpTxt;
    [SerializeField] TextMeshProUGUI _mpTxt;
    [SerializeField] TextMeshProUGUI _dashCooldownText;
    [SerializeField] Image _hpImage;
    [SerializeField] Image _mpImage;
    [SerializeField] Image _dashImage;

    private Coroutine _cooldownRoutine;

    private void Awake()
    {

    }
    private void Start()
    {
        _player.OnStatsChanged += UpdateStats;
        _dashBtn.onClick.AddListener(() => { GameManager.Instance.PlayerManager.Player.Dash(); });
    }
    private void UpdateStats(PlayerStat playerStat)
    { 
        _hpTxt.text = playerStat.GetStatValue(PlayerStatType.HP).ToString("F0") + " / " + playerStat.GetStatValue(PlayerStatType.MaxHP).ToString("F0");
        _mpTxt.text = playerStat.GetStatValue(PlayerStatType.MP).ToString("F0") + " / " + playerStat.GetStatValue(PlayerStatType.MaxMP).ToString("F0");
        _hpImage.fillAmount = playerStat.GetStatValue(PlayerStatType.HP) / playerStat.GetStatValue(PlayerStatType.MaxHP);
        _mpImage.fillAmount = playerStat.GetStatValue(PlayerStatType.MP) / playerStat.GetStatValue(PlayerStatType.MaxMP);
    }

    public void StartDashCooldown()
    {
        float dashCooldown = _player.GetStatValue(PlayerStatType.DashCooldown);

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
            _dashImage.color = new Color(_dashImage.color.r, _dashImage.color.g, _dashImage.color.b, 0.5f);
            _dashCooldownText.text = $"{remaining:F1}";
            remaining -= Time.deltaTime;
            yield return null;
            _dashImage.color = new Color(_dashImage.color.r, _dashImage.color.g, _dashImage.color.b, 1f);
        }
        _dashCooldownText.text = "";
    }
}

