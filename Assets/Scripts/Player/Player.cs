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
    [SerializeField] private PlayerSkillController playerSkillController;

    private void Awake()
    {
        _playerStat = GetComponent<PlayerStat>();
        _playerController = GetComponent<PlayerController>();
        playerSkillController= GetComponent<PlayerSkillController>();
    }
    private void Start()
    {
        _playerStat.InitBaseStat(statData);
    }
    public void Update()
    {
        _playerController.DirectionCheck();
    }
    public void Attack()
    {
        float attackSpeed = _playerStat.GetStatValue(PlayerStatType.AttackSpeed);
        if (Time.time - _lastHitTime < 1 / attackSpeed) return;
        //_playerController.StopMove();
        _playerController.SetTrigger("Attack");
        playerSkillController.UseSlashSkill(3);
        _lastHitTime = Time.time;
    }

    public void Dash()
    {
        _playerController.Dash();
    }
    public void ActiveSkill()
    {
        GameManager.Instance.SkillManager.ActiveSkill();
    }
    public void SetActiveSkilltrue()
    {
        GameManager.Instance.SkillManager.SetActiveSkilltrue();
    }
    public void SetActiveSkillfalse()
    {
         GameManager.Instance.SkillManager.SetActiveSkillfalse();
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
