using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CurrencyType
{
    Gold,
    Soul

}

public class CurrencyManager : MonoBehaviour
{
    public Dictionary<CurrencyType, int> currencies;
    public  Dictionary<CurrencyType, Action<int>> currenciesAction;
    public event Action<int> OnGoldChange;
    public event Action<int> OnSoulChange;
    
    public SaveManager saveManager;

    public static CurrencyManager Instance { get; private set; }

    private void Awake()
    {
        init();
    }

    private void Start()
    {
        GameData data = saveManager.LoadData();

        if (data.gold == 0 && data.soul == 0)
        {
            currencies[CurrencyType.Gold] = 1000;
            currencies[CurrencyType.Soul] = 100;

        }
        else
        {
            currencies[CurrencyType.Gold] = data.gold;
            currencies[CurrencyType.Gold] = data.soul;

        }

        OnGoldChange?.Invoke(currencies[CurrencyType.Gold]);
        OnGoldChange?.Invoke(currencies[CurrencyType.Soul]);

    }

    public void init()
    {
        currencies = new Dictionary<CurrencyType, int>
        {
            { CurrencyType.Gold, 0 },
            { CurrencyType.Soul,0}
        };

    }

    // 이벤트 액션 가져오기 (필요할 때마다 최신 이벤트 참조 반환)
    private Action<int> GetCurrencyAction(CurrencyType type)
    {
        switch (type)
        {
            case CurrencyType.Gold: return OnGoldChange;
            case CurrencyType.Soul: return OnSoulChange;
            default: return null;
        }
    }
    public bool AddCurrency(CurrencyType currencyType, int amount)
    {
        if (currencies.TryGetValue(currencyType, out int currency))
        {
            int AddCurrency = currency + amount;
            if (AddCurrency < 0)
            {
                Debug.Log($"{currencyType}의 값이 0보다 작습니다");
                return false;
            }

            currencies[currencyType] = AddCurrency;

            // 현재 이벤트 참조 가져와서 호출
            Action<int> action = GetCurrencyAction(currencyType);
            action?.Invoke(AddCurrency);

            return true;
        }
        else
        {
            Debug.Log($"{currencyType}이 잘못됬습니다. or {currencyType}값이 없습니다");
            return false;
        }
    }

    internal bool CanAfford(CurrencyType _currencyType, int _gold)
    {
        if (currencies[_currencyType] > _gold)
        {
            return true;
        }
        Debug.Log($"{currencies[_currencyType]}가 부족합니다");
        return false;

    }

    public void SaveCurrency()
    {
        GameData data = new GameData
        {
            gold = currencies[CurrencyType.Gold],
            soul = currencies[CurrencyType.Soul]

        };

        saveManager.SaveData(data);
    }

    private void OnApplicationQuit()
    {
        SaveCurrency();
    }
}
