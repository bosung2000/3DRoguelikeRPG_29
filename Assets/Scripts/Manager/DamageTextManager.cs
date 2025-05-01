using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageTextManager : MonoBehaviour
{
   
    [Header("설정")]
    [SerializeField] private GameObject _damageTextPrefab; // DamageText 프리팹
    [SerializeField] private int _poolSize = 30; // 풀 크기
    [SerializeField] private Canvas _worldSpaceCanvas; // 월드 스페이스 캔버스
    [SerializeField] private float _canvasScale = 0.01f; // 캔버스 스케일

    private Queue<GameObject> _damageTextPool; // 오브젝트 풀
    
    private void Awake()
    {
        InitializePool();
    }
    
    private void Start()
    {
       
    }
    
    // 초기화 메서드
    private void InitializePool()
    {
        // 만약 프리팹이 설정되지 않았다면 Resources에서 로드
        if (_damageTextPrefab == null)
        {
            _damageTextPrefab = Resources.Load<GameObject>("UI/DamageText");
            if (_damageTextPrefab == null)
            {
                Debug.LogError("DamageText 프리팹을 찾을 수 없습니다.");
                return;
            }
        }
        
        // 월드 스페이스 캔버스 초기화
        if (_worldSpaceCanvas == null)
        {
            // 캔버스 생성
            GameObject canvasObj = new GameObject("DamageTextCanvas");
            _worldSpaceCanvas = canvasObj.AddComponent<Canvas>();
            _worldSpaceCanvas.renderMode = RenderMode.WorldSpace;
            
            // 캔버스 스케일러 추가
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.dynamicPixelsPerUnit = 100;
            
            // 레이캐스터 추가
            canvasObj.AddComponent<GraphicRaycaster>();
            
            // 이 게임 오브젝트의 자식으로 설정
            canvasObj.transform.SetParent(transform);
            
            // 캔버스 크기 설정
            _worldSpaceCanvas.transform.localScale = new Vector3(_canvasScale, _canvasScale, _canvasScale);
        }
        
        // 오브젝트 풀 초기화
        _damageTextPool = new Queue<GameObject>();
        
        // 풀에 오브젝트 미리 생성
        for (int i = 0; i < _poolSize; i++)
        {
            GameObject textObj = CreateDamageTextObject();
            ReturnToPool(textObj);
        }
    }
    
    // 데미지 텍스트 오브젝트 생성
    private GameObject CreateDamageTextObject()
    {
        GameObject textObj = Instantiate(_damageTextPrefab, Vector3.zero, Quaternion.identity, _worldSpaceCanvas.transform);
        textObj.SetActive(false);
        
        return textObj;
    }
    
    // 풀에서 오브젝트 가져오기
    private GameObject GetFromPool()
    {
        if (_damageTextPool.Count == 0)
        {
            // 풀이 비어있으면 새로 생성
            return CreateDamageTextObject();
        }
        
        GameObject textObj = _damageTextPool.Dequeue();
        textObj.SetActive(true);
        return textObj;
    }
    
    // 풀에 오브젝트 반환
    private void ReturnToPool(GameObject textObj)
    {
        textObj.SetActive(false);
        _damageTextPool.Enqueue(textObj);
    }
    
    // 데미지 텍스트 표시 (공개 메서드)
    public void ShowDamageText(Vector3 worldPosition, float damage, bool isCritical)
    {
        // 캔버스가 카메라를 향하도록 설정
        _worldSpaceCanvas.transform.forward = Camera.main.transform.forward;
        
        // 위치 계산 (약간의 랜덤성 추가)
        Vector3 position = worldPosition + Vector3.up * 1.5f;
        position += new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(0f, 0.3f), 0);
        
        // 캔버스 위치 설정
        _worldSpaceCanvas.transform.position = position;
        
        // 풀에서 오브젝트 가져오기
        GameObject textObj = GetFromPool();
        
        // 로컬 위치/회전 초기화
        RectTransform rectTransform = textObj.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.localPosition = Vector3.zero;
            rectTransform.localRotation = Quaternion.identity;
        }
        
        // 텍스트 내의 모든 자식 오브젝트도 로컬 회전 초기화
        foreach (Transform child in textObj.transform)
        {
            child.localRotation = Quaternion.identity;
        }
        
        // 데미지 텍스트 설정
        DamageText damageText = textObj.GetComponent<DamageText>();
        if (damageText != null)
        {
            damageText.SetDamageText(damage, isCritical);
            damageText.SetReturnCallback(() => ReturnToPool(textObj));
        }
    }
} 