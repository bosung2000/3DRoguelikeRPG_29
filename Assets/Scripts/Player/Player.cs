using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public interface BaseEntity
{
    void MaxHPUp(float maxHP);
    void Healing(int heal);
    void MaxMPUp(float maxMP);
    void BaseMPUp(float currentMP);
    void SpeedUp(float speed);
    void TakeDamage(int damage);
    void Hit();
    void AttackUp(float attack);
    void DMGReductionUp(float damageReduction);
    void CriticalChanceUp(float criticalChance);
    void CriticalDamageUp(float criticalDamage);
    bool IsDead();
}

public class Player : MonoBehaviour, BaseEntity
{
    [SerializeField] private PlayerStatData statData;
    public PlayerStat _playerStat;

    [SerializeField] TestPlayerUI dashCooldownUI;
    [SerializeField] FloatingJoystick _floatingJoystick;
    [SerializeField] Rigidbody _rb;
    [SerializeField] LayerMask _obstacleLayer;
    private bool _isTumbling = false;
    private float _lastTumbleTime = -100f;

    private CurrencyManager currency;
    public CurrencyManager Currency => currency;

    private void Awake()
    {
        _playerStat = GetComponent<PlayerStat>();
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
        _playerStat.InitBaseStat(statData);
    }
    public void DirectionCheck()
    {
        Vector3 InputJoystick = Vector3.forward * _floatingJoystick.Vertical + Vector3.right * _floatingJoystick.Horizontal;

        Vector3 InputKeyboard = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        Vector3 inputDir = InputJoystick + InputKeyboard;

        if (inputDir.sqrMagnitude > 0.01f)
        {
            inputDir = inputDir.normalized;
            _rb.velocity = inputDir * _playerStat.GetStatValue(PlayerStatType.Speed);
        }
        else
        {
            _rb.velocity = Vector3.zero;
        }
    }
    public void FixedUpdate()
    {
        DirectionCheck();
    }

    public void MaxHPUp(float value)
    {
        _playerStat.ModifyStat(PlayerStatType.MaxHP, value);
        _playerStat.ModifyStat(PlayerStatType.HP, value);
    }
    public void Healing(int value)
    {
        float maxHP = _playerStat.GetStatValue(PlayerStatType.MaxHP);
        float currentHP = _playerStat.GetStatValue(PlayerStatType.HP);
        _playerStat.SetStatValue(PlayerStatType.HP, Mathf.Min(currentHP + value, maxHP));
    }
    public void MaxMPUp(float value)
    {
        _playerStat.ModifyStat(PlayerStatType.MaxMP, value);
        _playerStat.ModifyStat(PlayerStatType.MP, value);
    }
    public void BaseMPUp(float value)
    {
        float maxMP = _playerStat.GetStatValue(PlayerStatType.MaxMP);
        float currentMP = _playerStat.GetStatValue(PlayerStatType.MP);
        _playerStat.SetStatValue(PlayerStatType.MP, Mathf.Min(currentMP + value, maxMP));
    }
    public void SpeedUp(float speed)
    {
        //float currentSpeed = _stats.GetStatValue(PlayerStatType.Speed);
        //_stats.SetStatValue(PlayerStatType.Speed, currentSpeed + speed);
        _playerStat.ModifyStat(PlayerStatType.Speed, speed);
    }
    public void TakeDamage(int damage)
    {
        float currentHP = _playerStat.GetStatValue(PlayerStatType.HP);
        _playerStat.SetStatValue(PlayerStatType.HP, Mathf.Max(currentHP - damage, 0));
    }
    public void Hit()
    {
        float baseAttack = _playerStat.GetStatValue(PlayerStatType.Attack);
        float critChance = _playerStat.GetStatValue(PlayerStatType.CriticalChance);
        float critDamage = _playerStat.GetStatValue(PlayerStatType.CriticalDamage);

        bool isCrit = UnityEngine.Random.Range(0f, 100f) < critChance;
        float finalDamage = isCrit ? baseAttack * critDamage : baseAttack;

        Collider[] hits = Physics.OverlapSphere(transform.position, 2.5f); // 2.5f 범위 안의 적
        foreach (Collider col in hits)
        {
            BaseEntity enemy = col.GetComponent<BaseEntity>();

            if (enemy != null && !ReferenceEquals(enemy, this))
            {
                enemy.TakeDamage((int)finalDamage);
                Debug.Log($"{enemy}에게 {finalDamage} 데미지 ({(isCrit ? "CRI!" : "Normal")})");
            }
        }
    }
    public void AttackUp(float attack)
    {
        _playerStat.ModifyStat(PlayerStatType.Attack, attack);
    }

    public void DMGReductionUp(float damageReduction)
    {
        float currentDMGReduction = _playerStat.GetStatValue(PlayerStatType.DMGReduction);
        _playerStat.SetStatValue(PlayerStatType.DMGReduction, currentDMGReduction + damageReduction);
    }
    public void CriticalChanceUp(float criticalChance)
    {
        float currentCriticalChance = _playerStat.GetStatValue(PlayerStatType.CriticalChance);
        _playerStat.SetStatValue(PlayerStatType.CriticalChance, currentCriticalChance + criticalChance);
    }
    public void CriticalDamageUp(float criticalDamage)
    {
        float currentCriticalDamage = _playerStat.GetStatValue(PlayerStatType.CriticalDamage);
        _playerStat.SetStatValue(PlayerStatType.CriticalDamage, currentCriticalDamage + criticalDamage);
    }

    public void Dash()
    {
        float dashDistance = 5f;

        if (_isTumbling || Time.time < _lastTumbleTime + _playerStat.GetStatValue(PlayerStatType.DashCooltime))
        {
            Debug.Log("쿨타임입니다");
            return;
        }

        Vector3 joystickInput = new Vector3(_floatingJoystick.Horizontal, 0, _floatingJoystick.Vertical);
        Vector3 keyboardInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 dir = keyboardInput.sqrMagnitude > 0.01f ? keyboardInput : joystickInput;

        if (dir.sqrMagnitude < 0.01f)
        {
            dir = transform.forward;
        }

        dir = dir.normalized;

        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 target = transform.position + dir * dashDistance;

        if (Physics.Raycast(origin, dir, out RaycastHit hit, dashDistance, _obstacleLayer))
        {
            float safeDist = hit.distance - 0.05f; // 끼임방지
            target = transform.position + dir * safeDist;
        }

        StartCoroutine(TumbleRoutine(target));
        _lastTumbleTime = Time.time;
        dashCooldownUI.StartDashCooldown();
    }
    private IEnumerator TumbleRoutine(Vector3 target)
    {
        _isTumbling = true;

        Vector3 start = transform.position;
        float _elapsed = 0f;
        float duration = 0.2f;

        while (_elapsed < duration)
        {
            float t = _elapsed / duration;
            Vector3 newPos = Vector3.Lerp(start, target, t);
            _rb.MovePosition(newPos);

            _elapsed += Time.deltaTime;
            yield return null;
        }

        _rb.MovePosition(target);
        _rb.velocity = Vector3.zero;
        _isTumbling = false;
    }

    //public void Flash()
    //{
    //    if (Time.time >= lastFlashTime + 5)
    //    {
    //        Vector3 inputJoystick = Vector3.forward * _floatingJoystick.Vertical + Vector3.right * _floatingJoystick.Horizontal;
    //        Vector3 keyboardInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
    //        Vector3 dashDir = keyboardInput.sqrMagnitude > 0.01f ? keyboardInput : inputJoystick;

    //        if (dashDir.sqrMagnitude < 0.01f)
    //        {
    //            dashDir = transform.forward; // 입력 없을 시 정면
    //        }

    //        dashDir = dashDir.normalized;

    //        Vector3 origin = transform.position + Vector3.up * 0.5f;
    //        Vector3 targetPos = transform.position + dashDir * 5;

    //        if (!Physics.CapsuleCast(origin, origin, 0.3f, dashDir, out RaycastHit hit, 5f, _obstacleLayer))
    //        {
    //            _rb.MovePosition(targetPos);
    //            lastFlashTime = Time.time;
    //        }
    //    }
    //    else
    //    {
    //        Debug.Log("대쉬가 쿨타임입니다.");
    //    }
    //}

    public bool IsDead()
    {
        return _playerStat.GetStatValue(PlayerStatType.HP) <= 0f;
    }

    //public float GetCurrentHP()
    //{
    //    return _stats.GetStatValue(PlayerStatType.HP);
    //}

    //public void EquipItem(Item item)
    //{
    //    foreach (var statBonus in item.BaseEntity)
    //    {
    //        stats.AddEquipmentBonus(statBonus.type, statBonus.value);
    //    }
    //}
}
