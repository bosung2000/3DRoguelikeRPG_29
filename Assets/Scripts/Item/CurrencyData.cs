using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections;
using UnityEngine;


public class CurrencyData : MonoBehaviour
{
    [Header("재화 설정")]
    [SerializeField] CurrencyType currencType;

    private int _amount;
    /// <summary>
    /// 재화값 설정
    /// </summary>
    public void SetAmount(int amount)
    {
        _amount = amount;
    }

    public int GetAmount()
    {
        return _amount;
    }
}
