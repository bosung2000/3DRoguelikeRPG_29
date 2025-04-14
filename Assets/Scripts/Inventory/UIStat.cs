using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIStat : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Power;
    [SerializeField] private TextMeshProUGUI Mana;
    [SerializeField] private TextMeshProUGUI Health;
    [SerializeField] private TextMeshProUGUI Speed;
    [SerializeField] private TextMeshProUGUI Reduction;
    [SerializeField] private TextMeshProUGUI CriticalChance;
    [SerializeField] private TextMeshProUGUI CriticalDamage;

    PlayerStat playerStat;
    private void Awake()
    {
        playerStat = GameManager.Instance.PlayerManager.Player._playerStat;
        playerStat.OnStatsChanged += OnupdateStat;

    }

    private void OnEnable()
    {
        OnupdateStat(playerStat);
    }

    private void OnupdateStat(PlayerStat _playerStat)
    {
        Power.text = $"공격력 :{_playerStat.GetStatValue(PlayerStatType.Attack).ToString()}";
        Mana.text = $"최대마나 :{_playerStat.GetStatValue(PlayerStatType.MaxMP).ToString()}";
        Health.text = $"최대체력 :{_playerStat.GetStatValue(PlayerStatType.MaxHP).ToString()}";
        Speed.text = $"속도 :{_playerStat.GetStatValue(PlayerStatType.Speed).ToString()}";
        Reduction.text = $"피해감소 :{_playerStat.GetStatValue(PlayerStatType.DMGReduction).ToString()}";
        CriticalChance.text = $"크리티컬확률 :{_playerStat.GetStatValue(PlayerStatType.CriticalChance).ToString()}";
        CriticalDamage.text = $"크리티컬대미지 :{_playerStat.GetStatValue(PlayerStatType.CriticalDamage).ToString()}";
    }

}
