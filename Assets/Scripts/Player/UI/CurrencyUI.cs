using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyUI : MonoBehaviour
{
    [SerializeField] CurrencyManager _playerManager;
    [SerializeField] TextMeshProUGUI _GoldText;
    [SerializeField] TextMeshProUGUI _soulText;


    private void Start()
    {
        _playerManager.OnGoldChange += UpdateGold;
        _playerManager.OnSoulChange += UpdateSoul;
    }

    private void UpdateGold(int gold)
    {
        _GoldText.text = GameManager.Instance.PlayerManager.Currency.currencies[CurrencyType.Gold].ToString("F0");
    }
    private void UpdateSoul(int soul)
    {
        _soulText.text = GameManager.Instance.PlayerManager.Currency.currencies[CurrencyType.Soul].ToString("F0");
    }
}