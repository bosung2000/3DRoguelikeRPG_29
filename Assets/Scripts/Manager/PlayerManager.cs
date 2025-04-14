using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Player _player;
    public Player Player => _player;

    [SerializeField] private string playerName = "플레이어1";
    public string PlayerName => playerName;

    public event Action<string> OnPlayerNameChanged;

    private CurrencyManager currency;
    public CurrencyManager Currency => currency;

    public void SetPlayerName(string newName)
    {
        playerName = newName;
        OnPlayerNameChanged?.Invoke(playerName);
    }

    private void Awake()
    {
        currency = GetComponent<CurrencyManager>();
        // 골드 기본값 
        if (currency != null)
        {
            if (currency.currencies == null || currency.currencies.Count == 0)
            {
                currency.init();  //딕셔너리 강제 초기화
            }

            currency.AddCurrency(CurrencyType.Gold, 9000);  //골드추가
        }
    }

    private void Start()
    {
        init();
    }

    public void init()
    {
        //직접 넣어주자
        //_player = FindObjectOfType<Player>();
    }

}

