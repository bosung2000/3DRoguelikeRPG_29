using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _damageText;
    [SerializeField] private float _moveSpeed = 1.0f;
    [SerializeField] private float _fadeSpeed = 1.0f;
    [SerializeField] private float _duration = 1.0f;
    private Player player;

    private float _elapsedTime = 0f;
    private Color _textColor;

    private void Awake()
    {
        if (_damageText == null)
        {
            _damageText = GetComponentInChildren<TextMeshProUGUI>();
        }
        _textColor = _damageText.color;
    }

    private void Start()
    {
        player = FindObjectOfType<Player>();
        // 텍스트가 카메라를 향하도록 함
        LookAtCamera();

        // 지정된 시간 후 자동 삭제
        Destroy(gameObject, _duration);
    }

    private void Update()
    {
        // 위로 이동
        transform.Translate(Vector3.up * _moveSpeed * Time.deltaTime);

        // 페이드 아웃
        _elapsedTime += Time.deltaTime;
        float normalizedTime = _elapsedTime / _duration;

        _textColor.a = Mathf.Lerp(1f, 0f, normalizedTime * _fadeSpeed);
        _damageText.color = _textColor;

        // 항상 카메라를 향하도록
        LookAtCamera();
    }

    public void SetDamageText(float damage,bool CriticalBool)
    {
        if (_damageText != null)
        {
            _damageText.text = damage.ToString("0");

            // 크리티컬 여부에 따라 색상 변경 (선택적)
            bool isCritical = CriticalBool;
            if (isCritical)
            {
                _damageText.color = Color.red;
                _damageText.fontSize *= 1.2f;
                _textColor = _damageText.color;
            }
        }
    }

    private void LookAtCamera()
    {
        if (Camera.main != null)
        {
            transform.forward = Camera.main.transform.forward;
        }
    }
}