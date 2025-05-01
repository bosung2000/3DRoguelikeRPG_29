using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _damageText;
    [SerializeField] private float _moveSpeed = 1.0f;
    [SerializeField] private float _fadeSpeed = 1.0f;
    [SerializeField] private float _duration = 2.0f;

    private float _elapsedTime = 0f;
    private Color _textColor;
    private Vector3 _initialPosition;
    private Transform _canvasTransform;
    

    private void Awake()
    {
        if (_damageText == null)
        {
            _damageText = GetComponent<TextMeshProUGUI>();
        }
        _textColor = _damageText.color;
        _canvasTransform = transform.parent;
        _initialPosition = transform.localPosition;
    
    }

    private void Start()
    {
        // 로컬 회전 초기화
        transform.localRotation = Quaternion.identity;
        
        // TextMeshProUGUI의 회전도 초기화
        if (_damageText != null && _damageText.transform != transform)
        {
            _damageText.transform.localRotation = Quaternion.identity;
        }
        
        // 지정된 시간 후 자동 삭제
        Destroy(gameObject, _duration);
    }

    private void Update()
    {
        // 위로 이동 (로컬 좌표 기준)
        transform.localPosition = _initialPosition + Vector3.up * _moveSpeed * _elapsedTime;
        
        // 로컬 회전 유지
        transform.localRotation = Quaternion.identity;

        // 항상 카메라를 향하도록 설정 (부모 캔버스가 담당)
        if (_canvasTransform != null)
        {
            _canvasTransform.forward = Camera.main.transform.forward;
        }

        // 페이드 아웃
        _elapsedTime += Time.deltaTime;
        float normalizedTime = _elapsedTime / _duration;

        _textColor.a = Mathf.Lerp(1f, 0f, normalizedTime * _fadeSpeed);
        _damageText.color = _textColor;
    }

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
            
            // 텍스트 회전 초기화
            _damageText.transform.localRotation = Quaternion.identity;
        }
    }
}