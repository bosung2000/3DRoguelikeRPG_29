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
    private StatUI _statUI;

    private bool _isAttacking = false;
    private Vector3 _lastMoveDirection;
    Vector3 inputDir;
    

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        _playerStat = GetComponent<PlayerStat>();
        _floatingJoystick = FindObjectOfType<FloatingJoystick>();
        _statUI = FindObjectOfType<StatUI>();

        _lastMoveDirection = transform.forward;
        
        // Rigidbody 설정 추가
        if (_rb != null)
        {
            _rb.drag = 5f; // 수평 저항값 설정
            _rb.angularDrag = 100f; // 회전 저항값 설정
            _rb.interpolation = RigidbodyInterpolation.Interpolate; // 부드러운 움직임
        }
    }

    public void DirectionCheck()
    {
        if (_isAttacking) return;
        if(_isTumbling) return;


        Vector3 InputJoystick = Vector3.forward * _floatingJoystick.Vertical + Vector3.right * _floatingJoystick.Horizontal;

        Vector3 InputKeyboard = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        inputDir = InputJoystick + InputKeyboard;


        if (inputDir.sqrMagnitude > 0.05f)
        {
            inputDir = inputDir.normalized;
            _lastMoveDirection = inputDir;

            Quaternion targetRotation = Quaternion.LookRotation(inputDir);
            transform.rotation = targetRotation;

            // 목표 속도 계산
            Vector3 targetVelocity = new Vector3(
                inputDir.x * _playerStat.GetStatValue(PlayerStatType.MoveSpeed),
                _rb.velocity.y,
                inputDir.z * _playerStat.GetStatValue(PlayerStatType.MoveSpeed)
            );
            
            // 속도 직접 설정 (가속 없이)
            _rb.velocity = targetVelocity;
            
            SetBool("Run", true);
        }
        else
        {
            // 즉시 멈추기
            _rb.velocity = new Vector3(0, _rb.velocity.y, 0);
            _rb.angularVelocity = Vector3.zero;
            SetBool("Run", false);
        }
    }

    public void Dash()
    {
        _isAttacking = false;
        float dashDistance = _playerStat.GetStatValue(PlayerStatType.DashDistance);
        float dashCooldown = _playerStat.GetStatValue(PlayerStatType.DashCooldown);

        if (_isTumbling || Time.time < _lastTumbleTime + dashCooldown)
        {
            Debug.Log("쿨타임입니다");
            return;
        }

        _isTumbling = true;
        SetTrigger("Dash");

        Vector3 joystickInput = new Vector3(_floatingJoystick.Horizontal, 0, _floatingJoystick.Vertical);
        Vector3 keyboardInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 dir = keyboardInput.sqrMagnitude > 0.01f ? keyboardInput : joystickInput;

        if (dir.sqrMagnitude < 0.01f)
            dir = _lastMoveDirection;

        dir = dir.normalized;

        Vector3 origin = transform.position + Vector3.up * 0.01f;
        Vector3 target = transform.position + dir * dashDistance;

        if (_rb.SweepTest(dir, out RaycastHit hit, dashDistance))
        {
            float safeDist = Mathf.Max(hit.distance - 0.05f, 0f);
            target = transform.position + dir * safeDist;
        }

        float actualDistance = Vector3.Distance(transform.position, target);
        float dashDuration = actualDistance / dashDistance * 0.2f;

        StartCoroutine(TumbleRoutine(target, dashDuration));
        _lastTumbleTime = Time.time;
        _statUI.StartDashCooldown();
    }

    private IEnumerator TumbleRoutine(Vector3 target, float duration)
    {
        yield return new WaitForFixedUpdate();
        float DashSpeed = _anim.GetCurrentAnimatorClipInfo(0)[0].clip.length / duration;
        SetFloat("DashSpeed", DashSpeed);
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
        yield return new WaitUntil(() => _anim.GetCurrentAnimatorClipInfo(0)[0].clip.name != "Dash" || _anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1); 
        _isTumbling = false;
    }
    public void StopMove()
    {
        _rb.velocity = new Vector3(0, _rb.velocity.y, 0);
        _rb.angularVelocity = Vector3.zero;
        //_rb.rotation = Quaternion.LookRotation(inputDir);
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
