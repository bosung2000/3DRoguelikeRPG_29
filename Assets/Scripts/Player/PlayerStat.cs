using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public interface BaseEntity
{
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
    [SerializeField] PlayerStatData _statData;
    public PlayerStat playerStat;
    [SerializeField] PlayerController _playerController;
    [SerializeField] Weapon _Weapon;
    [SerializeField] private CameraShake _cameraShake;
    [SerializeField] private GameObject _dieMenu;
    [SerializeField] private GameObject[] _bloodEffect;
    [SerializeField] private Transform _bloodSpawnPoint;
    [SerializeField] private Transform _dieSpawnPoint;

    private float _lastHitTime = -100f;

    public event Action<PlayerStat> OnStatsChanged;

    private Dictionary<PlayerStatType, float> _equipmentBonuses = new Dictionary<PlayerStatType, float>();
    private Dictionary<PlayerStatType, float> _relicBonuses = new Dictionary<PlayerStatType, float>();
    private Dictionary<PlayerStatType, float> _buffBonuses = new Dictionary<PlayerStatType, float>();

    // 스킬 관련 변수
    private Dictionary<string, float> _skillCooldowns = new Dictionary<string, float>();
    private List<TimedBuff> _activeTimedBuffs = new List<TimedBuff>();
    private List<GameObject> activeEffects = new List<GameObject>();

    // 버프 지속시간 관리를 위한 클래스
    [System.Serializable]
    private class TimedBuff
    {
        public PlayerStatType statType;
        public float value;
        public float duration;
        public float remainingTime;

        public TimedBuff(PlayerStatType statType, float value, float duration)
        {
            this.statType = statType;
            this.value = value;
            this.duration = duration;
            this.remainingTime = duration;
        }
    }

    private void Awake()
    {
        playerStat = GetComponent<PlayerStat>();
        _playerController = GetComponent<PlayerController>();
        _Weapon = GetComponentInChildren<Weapon>();
        InitializeStats();
    }

    private void Start()
    {
        InitBaseStat(_statData);
        StartCoroutine(PeriodicRegen());
    }

    private void Update()
    {
        // 버프 지속시간 업데이트
        UpdateTimedBuffs();

        // 스킬 쿨다운 업데이트
        UpdateSkillCooldowns();
    }

    // 스킬 관련 메서드들 //

    /// <summary>
    /// 마나 사용 메서드
    /// </summary>
    /// <param name="amount">사용할 마나량</param>
    /// <returns>마나 사용 성공 여부</returns>
    public bool UseMana(float amount)
    {
        float currentMP = GetStatValue(PlayerStatType.MP);

        if (currentMP < amount)
            return false;

        SetStatValue(PlayerStatType.MP, currentMP - amount);
        return true;
    }
    private IEnumerator PeriodicRegen()
    {
        WaitForSeconds wait = new WaitForSeconds(10f);

        while (true)
        {
            yield return wait;

            float hpRecovery = GetStatValue(PlayerStatType.HPRecovery);
            float mpRecovery = GetStatValue(PlayerStatType.MPRecovery);

            RegenerateHP(hpRecovery);
            RegenerateMana(mpRecovery);
        }
    }
    public void RegenerateHP(float amount)
    {
        float currentHP = GetStatValue(PlayerStatType.HP);
        float maxHP = GetStatValue(PlayerStatType.MaxHP);

        SetStatValue(PlayerStatType.HP, Mathf.Min(currentHP + amount, maxHP));
    }
    /// <summary>
    /// 마나 회복 메서드
    /// </summary>
    /// <param name="amount">회복할 마나량</param>
    public void RegenerateMana(float amount)
    {
        float currentMP = GetStatValue(PlayerStatType.MP);
        float maxMP = GetStatValue(PlayerStatType.MaxMP);

        SetStatValue(PlayerStatType.MP, Mathf.Min(currentMP + amount, maxMP));
    }

    /// <summary>
    /// 특정 스킬의 쿨다운 시작
    /// </summary>
    /// <param name="skillName">스킬 이름</param>
    /// <param name="cooldownTime">쿨다운 시간</param>
    public void StartSkillCooldown(string skillName, float cooldownTime)
    {
        // 스킬 쿨타임 감소 적용
        float cooldownReduction = GetStatValue(PlayerStatType.SkillCooltime) / 100f;
        float finalCooldown = cooldownTime * (1f - cooldownReduction);

        _skillCooldowns[skillName] = finalCooldown;
    }

    /// <summary>
    /// 특정 스킬의 쿨다운 확인
    /// </summary>
    /// <param name="skillName">스킬 이름</param>
    /// <returns>남은 쿨다운 시간</returns>
    public float GetSkillCooldown(string skillName)
    {
        return _skillCooldowns.TryGetValue(skillName, out float value) ? value : 0f;
    }

    /// <summary>
    /// 스킬 쿨다운 업데이트
    /// </summary>
    private void UpdateSkillCooldowns()
    {
        List<string> finishedSkills = new List<string>();

        foreach (var pair in _skillCooldowns)
        {
            string skillName = pair.Key;
            float cooldown = pair.Value;

            cooldown -= Time.deltaTime;

            if (cooldown <= 0)
            {
                finishedSkills.Add(skillName);
            }
            else
            {
                _skillCooldowns[skillName] = cooldown;
            }
        }

        // 쿨다운이 끝난 스킬 제거
        foreach (var skill in finishedSkills)
        {
            _skillCooldowns.Remove(skill);
        }
    }

    /// <summary>
    /// 시간 제한이 있는 버프 추가
    /// </summary>
    /// <param name="statType">버프 적용할 스탯</param>
    /// <param name="value">증가량</param>
    /// <param name="duration">지속시간</param>
    public void AddBuff(PlayerStatType statType, float value, float duration)
    {
        // 임시 버프 추가
        _activeTimedBuffs.Add(new TimedBuff(statType, value, duration));

        // 버프 즉시 적용
        AddBuff(statType, value);
    }

    /// <summary>
    /// 시간 제한 버프 업데이트
    /// </summary>
    private void UpdateTimedBuffs()
    {
        if (_activeTimedBuffs.Count == 0)
            return;

        bool buffRemoved = false;

        for (int i = _activeTimedBuffs.Count - 1; i >= 0; i--)
        {
            var buff = _activeTimedBuffs[i];
            buff.remainingTime -= Time.deltaTime;

            if (buff.remainingTime <= 0)
            {
                // 버프 효과 제거
                AddBuff(buff.statType, -buff.value);
                _activeTimedBuffs.RemoveAt(i);
                buffRemoved = true;
            }
        }

        if (buffRemoved)
        {
            OnStatChanged();
        }
    }

    // 기존 코드와의 통합을 위한 메서드들 //

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
        _playerController.SetFloat("AttackSpeed", GetStatValue(PlayerStatType.AttackSpeed));
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
        float finalDamage = isCrit ? baseAttack * critDamage * 0.01f : baseAttack;
        enemy.TakeDamage(Mathf.RoundToInt(finalDamage), isCrit);
        absorp = Mathf.RoundToInt(finalDamage * absorp * 0.01f);
        Healing(absorp);

        if (isCrit)
            SoundManager.instance.PlayEffect(SoundEffectType.CriticalHit);
        else
            SoundManager.instance.PlayEffect(SoundEffectType.Hit);
        Debug.Log($"{enemy}에게 {finalDamage} 데미지 ({(isCrit ? "CRI!" : "Normal")})");
    }
    public void TakeDamage(int damage)
    {
        if (Time.time - _lastHitTime < GetStatValue(PlayerStatType.HitCooldown)) return;

        float currentHP = GetStatValue(PlayerStatType.HP);
        float damageReduction = GetStatValue(PlayerStatType.DMGReduction);
        float dmgIncrease = GetStatValue(PlayerStatType.DMGIncrease);

        int finalDamage = damage - Mathf.RoundToInt(damage * (damageReduction - dmgIncrease) / 100);
        _lastHitTime = Time.time;

        SetStatValue(PlayerStatType.HP, Mathf.Max(currentHP - finalDamage, 0));

        // 데미지 텍스트 표시
        if (DamageTextManager.Instance != null)
        {
            DamageTextManager.Instance.ShowDamageText(transform.position, finalDamage, false);
        }

        _cameraShake.ShakeAndDamage(2f, 0.3f,0.5f);

        SoundManager.instance.PlayEffect(SoundEffectType.TakeDamage);
        DieEffect(0,0,_bloodSpawnPoint,_bloodSpawnPoint,2);
        if (GetStatValue(PlayerStatType.HP) == 0)
        {
            //죽었을 때 저장
            GameManager.Instance?.PlayerManager?.Currency?.SaveCurrency();

            _playerController.SetTrigger("Die");
            DieEffect(3, 1, _dieSpawnPoint, _dieSpawnPoint, 6);
            Time.timeScale = 0f;
            StartCoroutine(PlayDeathAnimThenPauseGame());
            Debug.Log($"{gameObject.name}이(가) 사망했습니다.");
        }
        AnimatorStateInfo currentState = _playerController._anim.GetCurrentAnimatorStateInfo(0);
        if (currentState.IsName("Idle") || currentState.IsName("Run"))
        {
            _playerController.SetTrigger("GetHit");
        }
    }
    private IEnumerator PlayDeathAnimThenPauseGame()
    {
        AnimatorStateInfo state = _playerController._anim.GetCurrentAnimatorStateInfo(0);

        _playerController._anim.speed = 1f;

        yield return new WaitUntil(() => _playerController._anim.GetCurrentAnimatorStateInfo(0).IsName("Die"));

        yield return new WaitUntil(() => _playerController._anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

        float timer = 0f;
        while (timer < 0.5f)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        _dieMenu.SetActive(true);
    }
    //public void BloodEffect(int index, Transform position, Transform rotation, int duration)
    //{
    //    CleanupActiveEffects();

    //    GameObject effect = Instantiate(_bloodEffect[index], position.position, rotation.rotation);

    //    activeEffects.Add(effect);
    //    Destroy(effect, duration);
    //}

    public void DieEffect(float delay, int index, Transform position, Transform rotation, float duration)
    {
        CleanupActiveEffects();
        StartCoroutine(SpawnEffectAfterSeconds(delay, index, position, rotation, duration));
    }
    private IEnumerator SpawnEffectAfterSeconds(float delay, int index, Transform position, Transform rotation, float duration)
    {
        float elapsed = 0f;
        while (elapsed < delay)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        GameObject effect = Instantiate(_bloodEffect[index], position.position, rotation.rotation);
        activeEffects.Add(effect);

        // 생성 후 제거도 예약
        StartCoroutine(DestroyEffectAfterSeconds(effect, duration));
    }

    private IEnumerator DestroyEffectAfterSeconds(GameObject obj, float delay)
    {
        float elapsed = 0f;
        while (elapsed < delay)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        if (obj != null) Destroy(obj);
    }

    private void CleanupActiveEffects()
    {
        foreach (GameObject effect in activeEffects)
        {
            if (effect != null)
            {
                Destroy(effect);
            }
        }
        activeEffects.Clear();
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
        ModifyStat(PlayerStatType.SkillCooltime, skillColltime);
    }
    public void AttackSpeedUp(float attackSpeed)
    {
        ModifyStat(PlayerStatType.AttackSpeed, attackSpeed);
        _playerController.SetFloat("AttackSpeed", GetStatValue(PlayerStatType.AttackSpeed));
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
        _Weapon.EnableCollider();
    }

    public void DisableCollider()
    {
        _Weapon.DisableCollider();
    }

    public void LightingCamera()
    {
        _cameraShake.ShakeCamera(1, 1.5f);
        _cameraShake.ShakeCamera(3, 0.25f);
    }
    public void FireComboCamera()
    {
        _cameraShake.ShakeCamera(1, 2f);
        _cameraShake.ShakeCamera(3, 0.5f);
    }
    public void GreenSlashCamera()
    {
        _cameraShake.Zoom(45f, 1f);
        _cameraShake.ShakeCamera(2, 1f);
        _cameraShake.ShakeCamera(5, 0.2f);
    }
}