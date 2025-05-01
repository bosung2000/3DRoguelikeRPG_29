using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalfMoon : MeleeSkillBase
{
    [SerializeField] private float halfMoonRange = 2.5f; // 반달 공격 범위
    [SerializeField] private float halfMoonAngle = 180f; // 반달 공격 각도
    [SerializeField] private float halfMoonDamageMultiplier = 0.8f; // 데미지 배수 (넓은 범위 대신 데미지 감소)
    [SerializeField] private int maxTargets = 5; // 최대 타겟 수
    [SerializeField] private float sweepSpeed = 8f; // 휩쓸기 속도
    [SerializeField] private float criticalChanceBonus = 10f; // 크리티컬 확률 보너스
    [SerializeField] private float criticalDamageBonus = 50f; // 크리티컬 데미지 보너스

    protected override IEnumerator AttackCoroutine(Player player, Vector3 direction)
    {
        isAttacking = true;
        skillData.cooldown = skillData.maxCooldown;
        //player.GetComponent<PlayerController>().SetTrigger("HalfMoon");
        //yield return new WaitForSeconds(1);

        // 크리티컬 보너스 적용
        ApplyCriticalBonusToPlayer(player, true);

        // 방향 정규화
        direction = direction.normalized;

        // 반달 공격 시작 위치 (플레이어 앞, 약간 위로)
        Vector3 center = player.transform.position + Vector3.up * 0.5f;
        Vector3 forward = direction;

        // 반달 공격 범위 내의 적 탐색
        Collider[] hitColliders = Physics.OverlapSphere(center, halfMoonRange, targetLayer);
        
        // 각도 기반으로 적 정렬
        System.Array.Sort(hitColliders, (a, b) => 
        {
            float angleA = Vector3.Angle(forward, (a.transform.position - center).normalized);
            float angleB = Vector3.Angle(forward, (b.transform.position - center).normalized);
            return angleA.CompareTo(angleB);
        });

        int targetsHit = 0;
        foreach (var hitCollider in hitColliders)
        {
            if (targetsHit >= maxTargets) break;

            Vector3 dirToTarget = (hitCollider.transform.position - center).normalized;
            float angle = Vector3.Angle(forward, dirToTarget);
            
            if (angle <= halfMoonAngle * 0.5f)
            {
                Enemy enemy = hitCollider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    // 반달 특성 반영한 데미지 계산
                    int halfMoonDamage = Mathf.RoundToInt(skillData.value * halfMoonDamageMultiplier);
                    
                    // 크리티컬 적용
                    bool isCritical = IsAttackCritical(player);
                    if (isCritical)
                    {
                        float critMultiplier = GetCriticalDamageMultiplier(player);
                        halfMoonDamage = Mathf.RoundToInt(halfMoonDamage * critMultiplier);
                        ShowCriticalHitEffect(hitCollider.transform.position);
                    }
                    
                    enemy.TakeDamage(halfMoonDamage,isCritical);
                    ShowHalfMoonHitEffect(hitCollider.transform.position);
                    targetsHit++;
                }
            }
        }

        // 반달 공격 이펙트 표시
        ShowHalfMoonEffect(center, forward);

        yield return new WaitForSeconds(attackDelay);
        
        // 크리티컬 보너스 제거
        ApplyCriticalBonusToPlayer(player, false);
        
        isAttacking = false;
    }

    // 크리티컬 발생 확인
    private bool IsAttackCritical(Player player)
    {
        if (player == null || player._playerStat == null) 
            return false;
            
        float critChance = player._playerStat.GetStatValue(PlayerStatType.CriticalChance) / 100f;
        return Random.value <= critChance;
    }
    
    // 크리티컬 데미지 배율 구하기
    private float GetCriticalDamageMultiplier(Player player)
    {
        if (player == null || player._playerStat == null) 
            return 1.5f; // 기본 배율
            
        float critDamage = player._playerStat.GetStatValue(PlayerStatType.CriticalDamage) / 100f;
        return Mathf.Max(1.5f, critDamage); // 최소 1.5배
    }
    
    // 크리티컬 보너스 적용/제거 메서드
    private void ApplyCriticalBonusToPlayer(Player player, bool apply)
    {
        if (player == null || player._playerStat == null) return;
        
        if (apply)
        {
            // 현재 크리티컬 확률과 데미지에 보너스 적용
            player._playerStat.ModifyStat(PlayerStatType.CriticalChance, criticalChanceBonus);
            player._playerStat.ModifyStat(PlayerStatType.CriticalDamage, criticalDamageBonus);
        }
        else
        {
            // 보너스 제거
            player._playerStat.ModifyStat(PlayerStatType.CriticalChance, -criticalChanceBonus);
            player._playerStat.ModifyStat(PlayerStatType.CriticalDamage, -criticalDamageBonus);
        }
    }
    
    // 크리티컬 히트 이펙트
    private void ShowCriticalHitEffect(Vector3 position)
    {
        GameObject critEffect = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        position.y += 0.5f;
        critEffect.transform.position = position;
        critEffect.transform.localScale = Vector3.one * 0.8f;
        
        Destroy(critEffect.GetComponent<Collider>());
        
        Material material = new Material(Shader.Find("Standard"));
        material.color = new Color(1f, 0f, 0f, 1f); // 순수한 빨간색, 완전 불투명
        material.EnableKeyword("_EMISSION");
        material.SetColor("_EmissionColor", new Color(1f, 0f, 0f) * 5f); // 강한 빨간색 발광
        critEffect.GetComponent<Renderer>().material = material;
        
        // 확장 애니메이션
        StartCoroutine(AnimateCriticalEffect(critEffect));
    }
    
    // 크리티컬 이펙트 애니메이션
    private IEnumerator AnimateCriticalEffect(GameObject effect)
    {
        if (effect == null) yield break;
        
        float duration = 0.4f;
        float elapsed = 0f;
        float startScale = 0.8f;
        float endScale = 1.5f;
        
        while (elapsed < duration && effect != null)
        {
            float t = elapsed / duration;
            float scale = Mathf.Lerp(startScale, endScale, t);
            
            if (effect != null)
            {
                effect.transform.localScale = Vector3.one * scale;
                
                Renderer renderer = effect.GetComponent<Renderer>();
                if (renderer != null && renderer.material != null)
                {
                    renderer.material.color = new Color(1f, 0f, 0f, Mathf.Lerp(1f, 0f, t));
                }
            }
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        if (effect != null)
        {
            Destroy(effect);
        }
    }

    private void ShowHalfMoonEffect(Vector3 center, Vector3 direction)
    {
        // 반달 형태의 공격 범위 표시
        GameObject halfMoonEffect = new GameObject("HalfMoonEffect");
        halfMoonEffect.transform.position = center;
        halfMoonEffect.transform.rotation = Quaternion.LookRotation(direction);

        MeshFilter meshFilter = halfMoonEffect.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = halfMoonEffect.AddComponent<MeshRenderer>();

        // 반투명 재질 설정
        Material material = new Material(Shader.Find("Standard"));
        material.color = new Color(1f, 0.5f, 0f, 0.3f);
        material.EnableKeyword("_EMISSION");
        material.SetColor("_EmissionColor", new Color(1f, 0.5f, 0f) * 2f);
        meshRenderer.material = material;
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        meshRenderer.receiveShadows = false;

        // 반달 메시 생성
        meshFilter.mesh = CreateHalfMoonMesh(halfMoonRange, halfMoonAngle);

        // 휩쓸기 이펙트
        StartCoroutine(CreateSweepEffect(center, direction));

        Destroy(halfMoonEffect, 0.5f);
    }

    private Mesh CreateHalfMoonMesh(float radius, float angle)
    {
        Mesh mesh = new Mesh();

        int segments = 30;
        float halfAngle = angle * 0.5f * Mathf.Deg2Rad;

        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3];

        // 중앙 정점
        vertices[0] = Vector3.zero;

        // 반달 정점들
        for (int i = 0; i <= segments; i++)
        {
            float segmentAngle = -halfAngle + ((float)i / segments) * 2 * halfAngle;
            float x = Mathf.Sin(segmentAngle) * radius;
            float z = Mathf.Cos(segmentAngle) * radius;
            vertices[i + 1] = new Vector3(x, 0, z);
        }

        // 삼각형 인덱스
        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    private IEnumerator CreateSweepEffect(Vector3 center, Vector3 direction)
    {
        int sweepCount = 10;
        float interval = 0.05f;
        float sweepRadius = halfMoonRange * 0.8f;

        for (int i = 0; i < sweepCount; i++)
        {
            float progress = (float)i / sweepCount;
            float currentAngle = -halfMoonAngle * 0.5f + progress * halfMoonAngle;
            
            Vector3 sweepDir = Quaternion.Euler(0, currentAngle, 0) * direction;
            Vector3 sweepPos = center + sweepDir * sweepRadius;

            GameObject sweepEffect = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sweepEffect.transform.position = sweepPos;
            sweepEffect.transform.localScale = Vector3.one * 0.3f;

            Destroy(sweepEffect.GetComponent<Collider>());

            Material material = new Material(Shader.Find("Standard"));
            float alpha = 0.8f - (progress * 0.4f);
            material.color = new Color(1f, 0.5f, 0f, alpha);
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", new Color(1f, 0.5f, 0f) * 2f);
            sweepEffect.GetComponent<Renderer>().material = material;

            Destroy(sweepEffect, 0.2f);
            yield return new WaitForSeconds(interval);
        }
    }

    private void ShowHalfMoonHitEffect(Vector3 position)
    {
        // 반달 특유의 넓은 범위 히트 이펙트
        GameObject hitEffect = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        hitEffect.transform.position = position;
        hitEffect.transform.localScale = Vector3.one * 0.5f;

        Destroy(hitEffect.GetComponent<Collider>());

        Material material = new Material(Shader.Find("Standard"));
        material.color = new Color(1f, 0.5f, 0f, 0.8f);
        material.EnableKeyword("_EMISSION");
        material.SetColor("_EmissionColor", new Color(1f, 0.5f, 0f) * 3f);
        hitEffect.GetComponent<Renderer>().material = material;

        // 넓은 범위 파티클 효과
        StartCoroutine(CreateWideHitParticles(position));

        Destroy(hitEffect, 0.3f);
    }

    private IEnumerator CreateWideHitParticles(Vector3 position)
    {
        int particleCount = 12;
        float spreadAngle = 60f;
        float speed = 4f;
        float lifetime = 0.4f;

        for (int i = 0; i < particleCount; i++)
        {
            float angle = -spreadAngle * 0.5f + (spreadAngle * i / (particleCount - 1));
            Vector3 dir = Quaternion.Euler(0, angle, 0) * Vector3.forward;

            GameObject particle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            particle.transform.position = position;
            particle.transform.localScale = Vector3.one * 0.2f;

            Destroy(particle.GetComponent<Collider>());

            Material material = new Material(Shader.Find("Standard"));
            material.color = new Color(1f, 0.5f, 0f, 0.7f);
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", new Color(1f, 0.5f, 0f) * 2f);
            particle.GetComponent<Renderer>().material = material;

            StartCoroutine(MoveParticle(particle, position, dir, speed, lifetime));
            yield return new WaitForSeconds(0.01f);
        }
    }

    private IEnumerator MoveParticle(GameObject particle, Vector3 startPos, Vector3 direction, float speed, float lifetime)
    {
        float elapsed = 0f;

        while (elapsed < lifetime && particle != null)
        {
            particle.transform.position += direction * speed * Time.deltaTime;
            float scale = 0.2f * (1f - elapsed / lifetime);
            particle.transform.localScale = Vector3.one * scale;

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (particle != null)
        {
            Destroy(particle);
        }
    }
}
