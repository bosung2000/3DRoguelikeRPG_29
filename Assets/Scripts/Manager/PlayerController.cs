using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator _anim;
    private Rigidbody _rb;
    private PlayerStat _playerStat;
    private FloatingJoystick _floatingJoystick;

    private float _lastTumbleTime = -100f;
    private bool _isTumbling = false;
    private LayerMask _obstacleLayer;
    private TestPlayerUI dashCooldownUI;

    private bool _isAttacking = false;
    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        _playerStat = GetComponent<PlayerStat>();
        _floatingJoystick = FindObjectOfType<FloatingJoystick>();
        _obstacleLayer = LayerMask.GetMask("UI");
        dashCooldownUI = FindObjectOfType<TestPlayerUI>();
    }
    public void DirectionCheck()
    {
        if (_isAttacking) return;

        Vector3 InputJoystick = Vector3.forward * _floatingJoystick.Vertical + Vector3.right * _floatingJoystick.Horizontal;

        Vector3 InputKeyboard = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        Vector3 inputDir = InputJoystick + InputKeyboard;

        if (inputDir.sqrMagnitude > 0.05f)
        {
            inputDir = inputDir.normalized;

            Quaternion targetRotation = Quaternion.LookRotation(inputDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            float moveSpeed = _playerStat.GetStatValue(PlayerStatType.MoveSpeed);

            Vector3 velocity = inputDir * moveSpeed;
            velocity.y = _rb.velocity.y;
            _rb.velocity = velocity;

            SetBool("Run", true);
        }
        else
        {
            _rb.velocity = new Vector3(0, _rb.velocity.y, 0);
            SetBool("Run", false);
        }
    }
    public void Dash()
    {
        float dashDistance = _playerStat.GetStatValue(PlayerStatType.DashDistance);
        float dashCooldown = _playerStat.GetStatValue(PlayerStatType.DashCooldown);

        if (_isTumbling || Time.time < _lastTumbleTime + dashCooldown)
        {
            Debug.Log("쿨타임입니다");
            return;
        }
        SetTrigger("Dash");

        Vector3 joystickInput = new Vector3(_floatingJoystick.Horizontal, 0, _floatingJoystick.Vertical);
        Vector3 keyboardInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 dir = keyboardInput.sqrMagnitude > 0.01f ? keyboardInput : joystickInput;

        if (dir.sqrMagnitude < 0.01f)
            dir = transform.forward;

        dir = dir.normalized;

        Vector3 origin = transform.position + Vector3.up * 0.01f;
        Vector3 target = transform.position + dir * dashDistance;

        if (Physics.Raycast(origin, dir, out RaycastHit hit, dashDistance, _obstacleLayer))
        {
            float safeDist = Mathf.Max(hit.distance - 0.01f, 0f);
            target = transform.position + dir * safeDist;
        }

        float actualDistance = Vector3.Distance(transform.position, target);
        float dashDuration = actualDistance / dashDistance * 0.2f;

        StartCoroutine(TumbleRoutine(target, dashDuration));
        _lastTumbleTime = Time.time;
        dashCooldownUI.StartDashCooldown();
    }

    private IEnumerator TumbleRoutine(Vector3 target, float duration)
    {
        _isTumbling = true;

        Vector3 start = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            Vector3 newPos = Vector3.Lerp(start, target, t);
            _rb.MovePosition(newPos);

            elapsed += Time.deltaTime;
            yield return null;
        }

        _rb.MovePosition(target);
        _rb.velocity = Vector3.zero;
        _isTumbling = false;
    }

    public void SetBool(string name, bool value)
    {
        _anim.SetBool(name, value);
    }
    public void SetTrigger(string name)
    {
        if (!_anim.GetCurrentAnimatorStateInfo(0).IsName(name))
        {
            _anim.SetTrigger(name);
        }
    }
    public void SetFloat(string name, float value)
    {
        _anim.SetFloat(name, value);
    }
    public void Attacking()
    {
        _isAttacking = true;
    }
    public void NotAttacking()
    {
        _isAttacking = false;
    }
}
