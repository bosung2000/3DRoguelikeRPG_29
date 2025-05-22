using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum CurrencyType
{
    Gold,
    Soul

}

public class CurrencyManager : MonoBehaviour
{
    public Dictionary<CurrencyType, int> currencies;
    public Dictionary<CurrencyType, Action<int>> currenciesAction;
    public event Action<int> OnGoldChange;
    public event Action<int> OnSoulChange;

    public SaveManager saveManager;
    GameData data;
    public static CurrencyManager Instance { get; private set; }

    private void Awake()
    {
        saveManager = SaveManager.Instance;
        init();



    }

    private void Start()
    {
        data = saveManager.LoadData();

        //        Debug.Log($"Currencymamamger Start :data.gold:{data.gold} data.soul: {data.soul}");

        if (data.gold == 0 && data.soul == 0 && !data.isTutorialDone)
        {
            currencies[CurrencyType.Gold] = 500;
            currencies[CurrencyType.Soul] = 200;

        }
        else
        {
            currencies[CurrencyType.Gold] = data.gold;
            currencies[CurrencyType.Soul] = data.soul;

        }

        //Debug.Log($"Currencymamamger Start  end :data.gold:{data.gold} data.soul: {data.soul}");

        OnGoldChange?.Invoke(currencies[CurrencyType.Gold]);
        OnSoulChange?.Invoke(currencies[CurrencyType.Soul]);

        // 게임 씬에서만 튜토리얼 팝업 실행
        string currentScene = SceneManager.GetActiveScene().name;
        if (!data.isTutorialDone && (currentScene == "Bosung_02" || currentScene == "Main_02"))
        {
            var popup = UIManager.Instance.ShowPopupUI<UITutorialPrompt>();
            popup.tutorialStartPoint = GameObject.Find("potal_TutorialRoom")?.transform;
            popup.Initialize(data); // GameData 넘겨주기
        }

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
    public Action<int> GetCurrencyAction(CurrencyType type)
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
        if (currencies[_currencyType] >= _gold)
        {
            return true;
        }
        Debug.Log($"{currencies[_currencyType]}가 부족합니다");
        return false;

    }

    public void SaveCurrency()
    {
        data.gold = currencies[CurrencyType.Gold];
        data.soul = currencies[CurrencyType.Soul];

        //Debug.Log($"SaveCurrency :data.gold:{data.gold} data.soul: {data.soul}");
        //GameData data = new GameData
        //{
        //    gold = currencies[CurrencyType.Gold],
        //    soul = currencies[CurrencyType.Soul]

        //};

        saveManager.SaveData(data);
    }

    private void OnApplicationQuit()
    {
        SaveCurrency();
    }
    public void ResetCurrencyToInitial()
    {
        currencies[CurrencyType.Gold] = 500; // 초기 골드
        currencies[CurrencyType.Soul] = 200;  // 초기 소울

        OnGoldChange?.Invoke(currencies[CurrencyType.Gold]);
        OnSoulChange?.Invoke(currencies[CurrencyType.Soul]);

        SaveCurrency();
    }
}
