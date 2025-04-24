using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Player : MonoBehaviour
{
    [SerializeField] PlayerStatData statData;
    public PlayerStat _playerStat;
    [SerializeField] PlayerController _playerController;
    private float _lastHitTime = -100f;

    private void Awake()
    {
        _playerStat = GetComponent<PlayerStat>();
        _playerController = GetComponent<PlayerController>();
    }
    private void Start()
    {
        _playerStat.InitBaseStat(statData);
    }
    public void FixedUpdate()
    {
        _playerController.DirectionCheck();
    }


    public void MaxHPUp()
    {
        _playerStat.MaxHPUp(100);
    }
    public void Healing()
    {
        _playerStat.Healing(Mathf.RoundToInt(_playerStat.GetStatValue(PlayerStatType.MaxHP) * 0.2f));
        //최대체력의 20%  
    }
    public void MaxMPUp()
    {
        _playerStat.MaxMPUp(100);
    }
    public void BaseMPUp()
    {
        _playerStat.BaseMPUp(20);
    }
    public void SpeedUp()
    {
        _playerStat.MoveSpeedUp(5);
    }
    public void Attack()
    {
        float attackSpeed = _playerStat.GetStatValue(PlayerStatType.AttackSpeed);
        if (Time.time - _lastHitTime < 1 / attackSpeed) return;
        //_playerController.StopMove();
        _playerController.SetTrigger("Attack");
        _lastHitTime = Time.time;
    }
    public void AttackUp()
    {
        _playerStat.AttackUp(10);
    }

    public void DMGReductionUp()
    {
        _playerStat.DMGReductionUp(10);
    }

    public void CriticalChanceUp()
    {
        _playerStat.CriticalChanceUp(5);
        Debug.Log("CriticalChanceUp");
    }
    public void CriticalDamageUp()
    {
        _playerStat.CriticalDamageUp(0.25f);
    }
    public void DashDistanceUp()
    {
        _playerStat.DashDistanceUp(5);
    }
    public void DashCooldownUp()
    {
        _playerStat.DashCooldownUp(1);
    }
    public void HitCooldownUp()
    {
        _playerStat.HitCooldownUp(1);
    }
    public void DMGIncreaseUp()
    {
        _playerStat.DMGIncreaseUp(10);
    }
    public void HPRecoveryUp()
    {
        _playerStat.HPRecoveryUp(5);
    }
    public void MPRecoveryUp()
    {
        _playerStat.MPRecoveryUp(5);
    }
    public void GoldAcquisitionUp()
    {
        _playerStat.GoldAcquisitionUp(5);
    }
    public void SkillCooldownUp()
    {
        _playerStat.SkillCooldownUp(1);
    }
    public void AttackSpeedUp()
    {
        _playerStat.AttackSpeedUp(0.1f);
    }
    public void Dash()
    {
        _playerController.Dash();
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
}
