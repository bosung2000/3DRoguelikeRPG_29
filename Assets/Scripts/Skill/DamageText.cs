using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class DamageText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _damageText;
    [SerializeField] private float _moveSpeed = 1.0f;
    [SerializeField] private float _fadeSpeed = 1.0f;
    [SerializeField] private float _duration = 2.0f;

    private float _elapsedTime = 0f;
    private Color _textColor;
    
    private Action _returnToPoolCallback;
    private Coroutine _animationCoroutine;

    private void Awake()
    {
        if (_damageText == null)
        {
            _damageText = GetComponent<TextMeshProUGUI>();
        }
        _textColor = _damageText.color;
    }

    private void OnEnable()
    {
        // 오브젝트가 활성화될 때 초기화
        ResetText();
        
        // 애니메이션 코루틴 시작
        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
        }
        _animationCoroutine = StartCoroutine(AnimateText());
    }

    private void OnDisable()
    {
        // 애니메이션 코루틴 중지
        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
            _animationCoroutine = null;
        }
    }

    // 텍스트 초기화
    private void ResetText()
    {
        _elapsedTime = 0f;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        
        // TextMeshProUGUI 초기화
        if (_damageText != null)
        {
            _damageText.color = _textColor;
            _damageText.transform.localRotation = Quaternion.identity;
        }
    }

    // 애니메이션 코루틴
    private IEnumerator AnimateText()
    {
        _elapsedTime = 0f;
        
        while (_elapsedTime < _duration)
        {
            // 위로 이동 (로컬 좌표 기준)
            transform.localPosition = Vector3.up * _moveSpeed * _elapsedTime;
            
            // 로컬 회전 유지
            transform.localRotation = Quaternion.identity;

            // 페이드 아웃
            float normalizedTime = _elapsedTime / _duration;
            Color color = _textColor;
            color.a = Mathf.Lerp(1f, 0f, normalizedTime * _fadeSpeed);
            _damageText.color = color;

            _elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // 애니메이션 종료 후 풀에 반환
        ReturnToPool();
    }

    // 데미지 텍스트 설정
    public void SetDamageText(float damage, bool criticalBool)
    {
        if (_damageText != null)
        {
            _damageText.text = damage.ToString("0");

            // 크리티컬 여부에 따라 색상 변경
            if (criticalBool)
            {
                _damageText.color = Color.red;
                _damageText.fontSize *= 1.2f;
                _textColor = _damageText.color;
            }
            else
            {
                // 일반 데미지는 흰색으로
                _damageText.color = Color.white;
                _textColor = _damageText.color;
            }
            
            // 텍스트 회전 초기화
            _damageText.transform.localRotation = Quaternion.identity;
        }
    }
    
    // 풀에 반환하기 위한 콜백 설정
    public void SetReturnCallback(Action callback)
    {
        _returnToPoolCallback = callback;
    }
    
    // 풀에 오브젝트 반환
    private void ReturnToPool()
    {
        if (_returnToPoolCallback != null)
        {
            _returnToPoolCallback.Invoke();
        }
    }
}