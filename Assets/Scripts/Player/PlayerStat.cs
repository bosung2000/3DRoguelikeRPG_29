using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public interface BaseEntity
{
    void MaxHPUp(float maxHP);
    void Healing(float heal);
    void MaxMPUp(float maxMP);
    void BaseMPUp(float currentMP);
    void MoveSpeedUp(float speed);
    void TakeDamage(int damage);
    void AttackUp(float attack);
    void DMGReductionUp(float damageReduction);
    void CriticalChanceUp(float criticalChance);
    void CriticalDamageUp(float criticalDamage);
}
public class PlayerStat : BaseStat<PlayerStatType>, BaseEntity
{
    [SerializeField] PlayerStatData statData;
    public PlayerStat _playerStat;
    [SerializeField] PlayerController _playerController;
    [SerializeField] TestWeapon _testWeapon;

    private float _lastHitTime = -100f;

    public event Action<PlayerStat> OnStatsChanged;

    private Dictionary<PlayerStatType, float> _equipmentBonuses = new Dictionary<PlayerStatType, float>();
    private Dictionary<PlayerStatType, float> _relicBonuses = new Dictionary<PlayerStatType, float>();
    private Dictionary<PlayerStatType, float> _buffBonuses = new Dictionary<PlayerStatType, float>();
    //private Dictionary<PlayerStatType, float> _totalStats = new Dictionary<PlayerStatType, float>();
    private void Awake()
    {
        _playerStat = GetComponent<PlayerStat>();
        _playerController = GetComponent<PlayerController>();
        _testWeapon = GetComponentInChildren<TestWeapon>();
        InitializeStats();
    }
    private void Start()
    {
        InitBaseStat(statData);
    }
    /// <summary>
    /// 스탯 0으로 초기화
    /// </summary>
    protected override void InitializeStats()
    {
        //stas를 0으로 초기화 
        base.InitializeStats();
        foreach (PlayerStatType type in Enum.GetValues(typeof(PlayerStatType)))
        {
            _equipmentBonuses[type] = 0f;
            _relicBonuses[type] = 0f;
            _buffBonuses[type] = 0f;
        }
    }

    public void InitBaseStat(PlayerStatData playerStatData)
    {
        if (playerStatData != null)
        {
            // ScriptableObject에서 기본값을 가져와 초기화
            foreach (PlayerStatType type in System.Enum.GetValues(typeof(PlayerStatType)))
            {
                float baseValue = playerStatData.GetBaseValue(type);
                SetStatValue(type, baseValue);
            }
            OnStatChanged();
        }
        else
        {
            Debug.LogWarning("PlayerStatData 이 없습니다.");
        }
    }
    public override float GetStatValue(PlayerStatType type)
    {
        float baseValue = base.GetStatValue(type);
        float equipBonus = _equipmentBonuses.TryGetValue(type, out float equip) ? equip : 0f;
        float relicBonus = _relicBonuses.TryGetValue(type, out float relic) ? relic : 0f;
        float buffBonus = _buffBonuses.TryGetValue(type, out float buff) ? buff : 0f;

        return baseValue + equipBonus + relicBonus + buffBonus;
    }

    public void AddEquipmentBonus(Dictionary<PlayerStatType, float> totalconditionTypes)
    {
        foreach (var stat in totalconditionTypes)
        {
            _equipmentBonuses[stat.Key] += stat.Value;
        }
        OnStatChanged();
    }

    /// <summary>
    /// 유물 보너스 추가
    /// </summary>
    /// <param name="bonuses">적용할 유물 보너스</param>
    public void AddRelicBonus(Dictionary<PlayerStatType, float> bonuses)
    {
        foreach (var stat in bonuses)
        {
            _relicBonuses[stat.Key] += stat.Value;
        }
        OnStatChanged();
    }

    /// <summary>
    /// 특정 유물 보너스 확인
    /// </summary>
    /// <param name="type">확인할 스탯 타입</param>
    /// <returns>해당 스탯의 유물 보너스 값</returns>
    public float GetRelicBonus(PlayerStatType type)
    {
        return _relicBonuses.TryGetValue(type, out float value) ? value : 0f;
    }

    public void AddBuff(PlayerStatType type, float bonus)
    {
        if (!_buffBonuses.ContainsKey(type))
        {
            _buffBonuses[type] = 0f;
        }

        _buffBonuses[type] += bonus;
        OnStatChanged();
    }

    protected override void OnStatChanged()
    {
        base.OnStatChanged();
        OnStatsChanged?.Invoke(this);
    }
    /// <summary>
    /// 장비 보너스 스탯 초기화 
    /// </summary>
    internal void ClearEquipmentBonuses()
    {
        foreach (PlayerStatType type in Enum.GetValues(typeof(PlayerStatType)))
        {
            _equipmentBonuses[type] = 0f;
        }
        OnStatChanged();
    }

    /// <summary>
    /// 유물 보너스 스탯 초기화
    /// </summary>
    internal void ClearRelicBonuses()
    {
        foreach (PlayerStatType type in Enum.GetValues(typeof(PlayerStatType)))
        {
            _relicBonuses[type] = 0f;
        }
        OnStatChanged();
    }

    /// <summary>
    /// 모든 보너스 스탯 초기화 (장비, 유물, 버프)
    /// </summary>
    public void ClearAllBonuses()
    {
        foreach (PlayerStatType type in Enum.GetValues(typeof(PlayerStatType)))
        {
            _equipmentBonuses[type] = 0f;
            _relicBonuses[type] = 0f;
            _buffBonuses[type] = 0f;
        }
        OnStatChanged();
    }
    public void MaxHPUp(float value)
    {
        ModifyStat(PlayerStatType.MaxHP, value);
        ModifyStat(PlayerStatType.HP, value);
    }
    public void Healing(float value)
    {
        float maxHP = Mathf.RoundToInt(GetStatValue(PlayerStatType.MaxHP));
        float currentHP = Mathf.RoundToInt(GetStatValue(PlayerStatType.HP));
        SetStatValue(PlayerStatType.HP, Mathf.Min(currentHP + value, maxHP));
    }
    public void MaxMPUp(float value)
    {
        ModifyStat(PlayerStatType.MaxMP, value);
        ModifyStat(PlayerStatType.MP, value);
    }
    public void BaseMPUp(float value)
    {
        float maxMP = GetStatValue(PlayerStatType.MaxMP);
        float currentMP = GetStatValue(PlayerStatType.MP);
        SetStatValue(PlayerStatType.MP, Mathf.Min(currentMP + value, maxMP));
    }
    public void MoveSpeedUp(float speed)
    {
        ModifyStat(PlayerStatType.MoveSpeed, speed);
        //_playerController._anim.speed = (GetStatValue(PlayerStatType.MoveSpeed))*0.2f;
    }
    public void Attack(Enemy enemy)
    {
        float baseAttack = GetStatValue(PlayerStatType.Attack);
        float critChance = GetStatValue(PlayerStatType.CriticalChance);
        float critDamage = GetStatValue(PlayerStatType.CriticalDamage);
        float absorp = GetStatValue(PlayerStatType.absorp);

        bool isCrit = UnityEngine.Random.Range(0f, 100f) < critChance;
        float finalDamage = isCrit ? baseAttack * critDamage*0.01f : baseAttack;
        enemy.TakeDamage(Mathf.RoundToInt(finalDamage));
        absorp = Mathf.RoundToInt(finalDamage * absorp * 0.01f);
        Healing(absorp);

        Debug.Log($"{enemy}에게 {finalDamage} 데미지 ({(isCrit ? "CRI!" : "Normal")})");
    }
    public void TakeDamage(int damage)
    {
        if (Time.time - _lastHitTime < GetStatValue(PlayerStatType.HitCooldown)) return;

        float currentHP = GetStatValue(PlayerStatType.HP);
        float damageReduction = GetStatValue(PlayerStatType.DMGReduction);
        float dmgIncrease = GetStatValue(PlayerStatType.DMGIncrease);

        damage = damage - Mathf.RoundToInt(damage * (damageReduction - dmgIncrease) / 100);
        _lastHitTime = Time.time;

        SetStatValue(PlayerStatType.HP, Mathf.Max(currentHP - damage, 0));

        if (GetStatValue(PlayerStatType.HP) == 0)
        {
            _playerController.SetTrigger("Die");
            Time.timeScale = 0f;
            StartCoroutine(PlayDeathAnimThenPauseGame());
            Debug.Log($"{gameObject.name}이(가) 사망했습니다.");
        }
    }
    private IEnumerator PlayDeathAnimThenPauseGame()
    {
        AnimatorStateInfo state = _playerController._anim.GetCurrentAnimatorStateInfo(0);

        _playerController._anim.speed = 1f;

        yield return new WaitUntil(() => _playerController._anim.GetCurrentAnimatorStateInfo(0).IsName("Die"));

        yield return new WaitUntil(() => _playerController._anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

    }

    public void AttackUp(float attack)
    {
        ModifyStat(PlayerStatType.Attack, attack);
    }

    public void DMGReductionUp(float damageReduction)
    {
        ModifyStat(PlayerStatType.DMGReduction, damageReduction);
    }
    public void CriticalChanceUp(float criticalChance)
    {
        ModifyStat(PlayerStatType.CriticalChance, criticalChance);
    }
    public void CriticalDamageUp(float criticalDamage)
    {
        ModifyStat(PlayerStatType.CriticalDamage, criticalDamage);
    }
    public void DashDistanceUp(float dashDistance)
    {
        ModifyStat(PlayerStatType.DashDistance, dashDistance);
    }
    public void DashCooldownUp(float dashCooldown)
    {
        ModifyStat(PlayerStatType.DashCooldown, dashCooldown);
    }
    public void HitCooldownUp(float hitCooldown)
    {
        ModifyStat(PlayerStatType.HitCooldown, hitCooldown);
    }
    public void absorpUp(float absorp)
    {
        ModifyStat(PlayerStatType.absorp, absorp);
    }
    public void DMGIncreaseUp(float damageIncrease)
    {
        ModifyStat(PlayerStatType.DMGIncrease, damageIncrease);
    }
    public void HPRecoveryUp(float hpRecovery)
    {
        ModifyStat(PlayerStatType.HPRecovery, hpRecovery);
    }
    public void MPRecoveryUp(float mpRecovery)
    {
        ModifyStat(PlayerStatType.MPRecovery, mpRecovery);
    }
    public void GoldAcquisitionUp(float goldAcquisition)
    {
        ModifyStat(PlayerStatType.GoldAcquisition, goldAcquisition);
    }
    public void SkillCooldownUp(float skillColltime)
    {
        ModifyStat(PlayerStatType.SkillColltime, skillColltime);
    }
    public void AttackSpeedUp(float attackSpeed)
    {
        ModifyStat(PlayerStatType.AttackSpeed, attackSpeed);
        _playerController._anim.speed = (GetStatValue(PlayerStatType.AttackSpeed)) / 5;
    }

    private void OnCollisionEnter(Collision other)
    {
        CurrencyData currencyData = other.gameObject.GetComponent<CurrencyData>();

        if (other.gameObject.CompareTag("Gold"))
        {
            GameManager.Instance.PlayerManager.Currency.AddCurrency(CurrencyType.Gold, currencyData._amount);
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("Soul"))
        {
            GameManager.Instance.PlayerManager.Currency.AddCurrency(CurrencyType.Soul, currencyData._amount);
            Destroy(other.gameObject);
        }
    }
    public void EnableCollider()
    {
        _testWeapon.EnableCollider();
    }

    public void DisableCollider()
    {
        _testWeapon.DisableCollider();
    }
}