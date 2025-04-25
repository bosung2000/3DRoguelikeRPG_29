using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkillProjectile : MonoBehaviour
{
    [SerializeField] private float currentDuration; //지나갈 시간
    [SerializeField] private Vector3 direction; //방향
    [SerializeField] private float Duration; //투사체 지속시간
    [SerializeField] private float ProjectileSpeed; //투사체 속도
    public LayerMask target; //충돌했을때 읽어올 레이어 (적, 벽 등 모든 충돌 대상 포함)
    public bool fxOnDestroy; //없어질 때 이펙트를 호출하는 투사체인지

    // 데미지 관련 변수 추가
    [HideInInspector] public int damage = 0; // 투사체가 적중 시 입힐 데미지
    [SerializeField] private float criticalChance = 0.1f; // 크리티컬 확률 (10%)
    [SerializeField] private float criticalMultiplier = 1.5f; // 크리티컬 데미지 배율 (150%)
    [SerializeField] private GameObject impactEffectPrefab; // 적중 효과 프리팹

    // 투사체 특성 (선택적)
    [SerializeField] private bool isPenetrating = false; // 관통 여부
    [SerializeField] private int penetrationCount = 0; // 관통 횟수 (0은 관통 안함)
    [SerializeField] private bool isHoming = false; // 유도 미사일 여부
    [SerializeField] private float homingSpeed = 5.0f; // 유도 회전 속도
    [SerializeField] private float homingRadius = 10.0f; // 유도 감지 범위

    // 특수 효과
    [SerializeField] private bool hasSplashDamage = false; // 폭발 여부
    [SerializeField] private float splashRadius = 3.0f; // 폭발 범위
    [SerializeField] private float splashDamageRatio = 0.5f; // 폭발 데미지 비율 (기본 데미지의 50%)
    [SerializeField] private GameObject explosionEffectPrefab; // 폭발 효과 프리팹

    private int hitCount = 0; // 적중 횟수 카운트
    private Transform homingTarget = null; // 유도 대상

    private void Awake()
    {
    }

    private void Update()
    {
        //투사체의 시간을 흐르게 한 뒤
        currentDuration += Time.deltaTime;

        //투사체의 지속시간보다 더 오래 지속되었을 경우, 오브젝트 삭제
        if (currentDuration > Duration)
        {
            DestroyProjectile(transform.position, false);
        }

        // 유도 미사일 로직
        if (isHoming && homingTarget == null)
        {
            FindHomingTarget();
        }
        else if (isHoming && homingTarget != null)
        {
            // 타겟 방향으로 회전
            Vector3 targetDirection = (homingTarget.position - transform.position).normalized;
            direction = Vector3.Slerp(direction, targetDirection, homingSpeed * Time.deltaTime);
            transform.forward = direction; // forward 방향으로 설정 (right 대신)
        }

        // 이동 방향을 forward로 변경하여 일관성 있게 처리
        transform.position += transform.forward * ProjectileSpeed * Time.deltaTime;

        // 디버그용 - 실제 투사체 이동 방향 시각화
        Debug.DrawRay(transform.position, transform.forward * 2f, Color.red);
    }

    private void OnTriggerEnter(Collider collision)
    {
        // 충돌 대상이 target 레이어에 포함되어 있는지 확인
        if (target.value == (target.value | (1 << collision.gameObject.layer)))
        {
            // 적인지 확인
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                // 적에게 데미지 적용
                ApplyDamageToEnemy(enemy);
                
                // 관통 로직
                hitCount++;
                if (isPenetrating && hitCount <= penetrationCount)
                {
                    // 관통 효과 표시만 하고 파괴하지 않음
                    if (impactEffectPrefab != null)
                    {
                        GameObject impact = Instantiate(impactEffectPrefab, collision.ClosestPoint(transform.position), Quaternion.identity);
                        Destroy(impact, 2f);
                    }
                    
                    // 폭발 데미지 적용 (관통 시에도)
                    if (hasSplashDamage)
                    {
                        ApplySplashDamage(collision.ClosestPoint(transform.position));
                    }
                    
                    // 관통이므로 파괴하지 않고 계속 진행
                    return;
                }
            }
            
            // 폭발 효과 적용 (적 또는 벽에 부딪힐 때)
            if (hasSplashDamage)
            {
                ApplySplashDamage(collision.ClosestPoint(transform.position));
            }
            
            // 충돌 지점에서 이펙트 생성 후 투사체 제거
            Vector3 hitPoint = collision.ClosestPoint(transform.position);
            if (enemy == null) // 벽이나 다른 장애물인 경우
            {
                hitPoint -= direction * 0.2f; // 벽 관통 방지용 약간 후퇴
            }
            
            DestroyProjectile(hitPoint, fxOnDestroy);
        }
    }

    // 적에게 데미지 적용
    private void ApplyDamageToEnemy(Enemy enemy)
    {
        // 크리티컬 판정
        bool isCritical = Random.value <= criticalChance;
        int damageAmount = damage;

        // 크리티컬 히트 시 데미지 증가
        if (isCritical)
        {
            damageAmount = Mathf.RoundToInt(damage * criticalMultiplier);
        }

        // 데미지 적용
        enemy.TakeDamage(damageAmount);

        // 크리티컬 효과 (선택적)
        if (isCritical && impactEffectPrefab != null)
        {
            GameObject impact = Instantiate(impactEffectPrefab, enemy.transform.position, Quaternion.identity);
            // 크리티컬 효과 크기 조정 (예시)
            impact.transform.localScale *= 1.5f;
            Destroy(impact, 2f);
        }
    }

    // 범위 데미지 적용
    private void ApplySplashDamage(Vector3 center)
    {
        // 폭발 이펙트 생성
        if (explosionEffectPrefab != null)
        {
            GameObject explosion = Instantiate(explosionEffectPrefab, center, Quaternion.identity);
            Destroy(explosion, 2f);
        }

        // 범위 내 적 탐색
        Collider[] hitColliders = Physics.OverlapSphere(center, splashRadius, target);

        foreach (var hitCollider in hitColliders)
        {
            Enemy enemy = hitCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                // 거리에 따른 데미지 감소 (선택적)
                float distance = Vector3.Distance(center, hitCollider.transform.position);
                float damageRatio = 1 - (distance / splashRadius); // 거리가 멀수록 데미지 감소
                damageRatio = Mathf.Clamp(damageRatio, 0.1f, 1f) * splashDamageRatio;

                int splashDamage = Mathf.RoundToInt(damage * damageRatio);
                enemy.TakeDamage(splashDamage);
            }
        }
    }

    // 유도 대상 찾기
    private void FindHomingTarget()
    {
        // 일정 범위 내 적 검색
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, homingRadius, target);

        if (hitColliders.Length > 0)
        {
            // 가장 가까운 적 선택
            float closestDistance = float.MaxValue;
            foreach (var hitCollider in hitColliders)
            {
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    homingTarget = hitCollider.transform;
                }
            }
        }
    }

    public void Init(Vector3 _direction, int _projectileSpeed)
    {
        // 방향 벡터 정규화
        this.direction = _direction.normalized;
        this.ProjectileSpeed = _projectileSpeed;
        currentDuration = 0;

        // 방향 벡터에 맞게 오브젝트 회전 설정
        // 이전 코드: transform.right = this.direction
        transform.forward = this.direction; // 앞쪽 방향을 이동 방향으로 설정

        // 속도가 0인 경우 기본값 설정
        if (ProjectileSpeed <= 0)
        {
            ProjectileSpeed = 10f; // 기본 속도 값
            Debug.LogWarning("ProjectileSpeed was 0 or negative, set to default 10");
        }
    }

    private void DestroyProjectile(Vector3 position, bool createFx)
    {
        if (createFx)
        {
            // 충돌 효과 생성
            if (impactEffectPrefab != null)
            {
                GameObject impact = Instantiate(impactEffectPrefab, position, Quaternion.identity);
                Destroy(impact, 2f);
            }
        }
        Destroy(this.gameObject);
    }

    // 투사체 속성을 한 번에 설정하는 메소드 추가
    public void ConfigureProjectile(bool isPenetrating, int penetrationCount, bool isHoming, bool hasSplashDamage)
    {
        this.isPenetrating = isPenetrating;
        this.penetrationCount = penetrationCount;
        this.isHoming = isHoming;
        this.hasSplashDamage = hasSplashDamage;
    }

    // 개별 속성 설정 메소드들
    public void SetPenetrating(bool isPenetrating, int count = 3)
    {
        this.isPenetrating = isPenetrating;
        this.penetrationCount = count;
    }

    public void SetHoming(bool isHoming, float speed = 5.0f, float radius = 10.0f)
    {
        this.isHoming = isHoming;
        this.homingSpeed = speed;
        this.homingRadius = radius;
    }

    public void SetSplashDamage(bool hasSplash, float radius = 3.0f, float damageRatio = 0.5f)
    {
        this.hasSplashDamage = hasSplash;
        this.splashRadius = radius;
        this.splashDamageRatio = damageRatio;
    }

    private void SetDirection(Vector3 _direction)
    {
        this.direction = _direction;
    }

    // 속성 접근자 (읽기 전용)
    public bool IsPenetrating => isPenetrating;
    public int PenetrationCount => penetrationCount;
    public bool IsHoming => isHoming;
    public bool HasSplashDamage => hasSplashDamage;
}