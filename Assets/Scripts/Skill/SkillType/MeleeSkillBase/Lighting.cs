using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lighting : MeleeSkillBase
{
    
    [SerializeField] private int maxTargets = 10; // 최대 타겟 수
    [SerializeField] private float criticalChanceBonus = 10f; // 크리티컬 확률 보너스
    [SerializeField] private float criticalDamageBonus = 50f; // 크리티컬 데미지 보너스
    [SerializeField] private float spinDuration = 1.5f; // 회전 지속 시간
    [SerializeField] private float spinInterval = 0.2f; // 회전 데미지 간격
    [SerializeField] private float spinRange = 3f; // 회전 공격 범위
    [SerializeField] private float spinDamageRate = 20; // 회전 데미지 %
    [SerializeField] private float slashRange = 4f; // 참격 범위
    [SerializeField] private float slashAngle = 130f; // 참격 각도
    [SerializeField] private float FinishslashDamageRate = 100; // 참격 데미지 %

    protected override IEnumerator AttackCoroutine(Player player, Vector3 direction)
    {
        isAttacking = true;
        skillData.cooldown = skillData.maxCooldown;


        // 1. 스핀 어택 단계
        float elapsed = 0f;
        while (elapsed < spinDuration)
        {
            // 플레이어 회전 애니메이션 트리거 (필요시 구현)
            DealSpinDamage(player);
            //ShowSpinEffect(player.transform.position);
            yield return new WaitForSeconds(spinInterval);
            elapsed += spinInterval;
        }
        //yield return new WaitForSeconds(0.2f);

        // 2. 피니시 슬래시 단계
        FinishSlash(player, direction);
        

        yield return new WaitForSeconds(attackDelay);
        isAttacking = false;
    }

    private void FinishSlash(Player player, Vector3 direction)
    {
        //float SkillRate = (float)((float)skillData.value / (float)100);
        float SkillRate = FinishslashDamageRate/100f;
        // 기본 데미지
        int FinishSlashDamage = (int)(player._playerStat.GetStatValue(PlayerStatType.Attack) * SkillRate);

        // 크리티컬 적용
        bool isCritical = IsAttackCritical(player);
        if (isCritical)
        {
            float critMultiplier = GetCriticalDamageMultiplier(player);
            FinishSlashDamage = Mathf.RoundToInt(FinishSlashDamage * critMultiplier);
        }

        HashSet<Enemy> alreadyHitEnemies = new HashSet<Enemy>();
        Vector3 dir = direction.normalized;
        GameObject projObj = GameObject.CreatePrimitive(PrimitiveType.Capsule); // 임시 프리팹
        projObj.transform.position = player.transform.position + Vector3.up * 0.5f;
        projObj.transform.localScale = new Vector3(slashRange, 0.3f, slashRange * 0.5f); // 반달 느낌의 크기
        projObj.transform.rotation = Quaternion.LookRotation(dir);
        var collider = projObj.GetComponent<Collider>();
        if (collider != null) collider.isTrigger = true;
        // Renderer만 삭제 (이렇게 하면 화면에 안 보임)
        var renderer = projObj.GetComponent<Renderer>();
        if (renderer != null) Destroy(renderer);
        var proj = projObj.AddComponent<LightingSlashProjectile>();
        proj.Init(dir, FinishSlashDamage, 20f, 0.5f, alreadyHitEnemies);
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

    // 회전 어택 데미지 함수
    private void DealSpinDamage(Player player)
    {
        //float SkillRate = (float)((float)skillData.value / (float)100);
        float SkillRate = spinDamageRate/ 100f;
        // 기본 데미지
        int SpinDamage = (int)(player._playerStat.GetStatValue(PlayerStatType.Attack) * SkillRate);

        // 크리티컬 적용
        bool isCritical = IsAttackCritical(player);
        if (isCritical)
        {
            float critMultiplier = GetCriticalDamageMultiplier(player);
            SpinDamage = Mathf.RoundToInt(SpinDamage * critMultiplier);
        }

        Collider[] hitColliders = Physics.OverlapSphere(player.transform.position, spinRange, targetLayer);
        foreach (var hitCollider in hitColliders)
        {
            Enemy enemy = hitCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(SpinDamage, false);
            }
        }
    }

    

    // 회전 이펙트 함수 (간단한 예시)
    private void ShowSpinEffect(Vector3 position)
    {
        GameObject spinEffect = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        spinEffect.transform.position = position + Vector3.up * 0.1f;
        spinEffect.transform.localScale = new Vector3(spinRange * 2f, 0.05f, spinRange * 2f);
        Destroy(spinEffect.GetComponent<Collider>());
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = new Color(0.5f, 0.7f, 1f, 0.3f);
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", new Color(0.5f, 0.7f, 1f) * 2f);
        spinEffect.GetComponent<Renderer>().material = mat;
        Destroy(spinEffect, 0.2f);
    }

    // 참격 이펙트 함수 (간단한 예시)
    private void ShowSlashEffect(Vector3 position, Vector3 direction)
    {
        GameObject slashEffect = new GameObject("SlashEffect");
        slashEffect.transform.position = position + Vector3.up * 0.2f;
        slashEffect.transform.rotation = Quaternion.LookRotation(direction);
        MeshFilter meshFilter = slashEffect.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = slashEffect.AddComponent<MeshRenderer>();
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = new Color(1f, 1f, 0.3f, 0.4f);
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", new Color(1f, 1f, 0.3f) * 3f);
        meshRenderer.material = mat;
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        meshRenderer.receiveShadows = false;
        //meshFilter.mesh = CreateHalfMoonMesh(slashRange, slashAngle);
        Destroy(slashEffect, 0.5f);
    }
}
