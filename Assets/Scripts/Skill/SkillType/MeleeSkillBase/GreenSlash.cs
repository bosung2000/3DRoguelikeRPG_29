using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenSlash : MeleeSkillBase
{
    [SerializeField] private float aoeRange = 4f; // 범위 반경
    [SerializeField] private float criticalChanceBonus = 20f; // 크리티컬 확률 보너스
    [SerializeField] private float criticalDamageBonus = 30f; // 크리티컬 데미지 보너스
    [SerializeField] private PlayerController _playerController;


    protected override IEnumerator AttackCoroutine(Player player, Vector3 direction)
    {
        isAttacking = true;
        skillData.cooldown = skillData.maxCooldown;
        //ApplyCriticalBonusToPlayer(player, true);

        Vector3 center = player.transform.position + Vector3.up * 0.5f;
        Collider[] hitColliders = Physics.OverlapSphere(center, aoeRange, targetLayer);

        foreach (var hitCollider in hitColliders)
        {
            Enemy enemy = hitCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                float SkillRate = (float)((float)skillData.value / (float)100);
                int aoeDamage = (int)(player._playerStat.GetStatValue(PlayerStatType.Attack) * SkillRate);

                bool isCritical = IsAttackCritical(player);
                if (isCritical)
                {
                    float critMultiplier = GetCriticalDamageMultiplier(player);
                    aoeDamage = Mathf.RoundToInt(aoeDamage * critMultiplier);
                    //ShowCriticalHitEffect(enemy.transform.position);
                }

                enemy.TakeDamage(aoeDamage, isCritical);
                //ShowAoeHitEffect(enemy.transform.position);
            }
        }

        //ShowAoeEffect(center, aoeRange);

        yield return new WaitForSeconds(attackDelay);
        //ApplyCriticalBonusToPlayer(player, false);
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

        // 크리티컬 파티클 효과
        StartCoroutine(CreateCriticalParticles(position));

        // 이펙트 자동 파괴 추가
        Destroy(critEffect, 0.5f);
    }

    private IEnumerator CreateCriticalParticles(Vector3 position)
    {
        int particleCount = 15;
        float speed = 8f;
        float lifetime = 0.4f;

        for (int i = 0; i < particleCount; i++)
        {
            Vector3 randomDir = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(0.2f, 1f),
                Random.Range(-1f, 1f)
            ).normalized;

            GameObject particle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            if (particle != null)
            {
                particle.transform.position = position;
                particle.transform.localScale = Vector3.one * 0.25f;

                Destroy(particle.GetComponent<Collider>());

                Material material = new Material(Shader.Find("Standard"));
                material.color = new Color(1f, 0f, 0f, 1f);
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", new Color(1f, 0f, 0f) * 5f);

                Renderer renderer = particle.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material = material;
                }

                StartCoroutine(MoveParticle(particle, position, randomDir, speed, lifetime));
            }

            yield return new WaitForSeconds(0.01f);
        }
    }

    private void ShowAoeEffect(Vector3 center, float range)
    {
        GameObject aoeEffect = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        aoeEffect.transform.position = center;
        aoeEffect.transform.localScale = new Vector3(range * 2f, 0.05f, range * 2f);
        Destroy(aoeEffect.GetComponent<Collider>());
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = new Color(0.2f, 1f, 0.2f, 0.3f);
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", new Color(0.2f, 1f, 0.2f) * 2f);
        aoeEffect.GetComponent<Renderer>().material = mat;
        Destroy(aoeEffect, 0.3f);
    }

    private void ShowAoeHitEffect(Vector3 position)
    {
        GameObject hitEffect = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        hitEffect.transform.position = position;
        hitEffect.transform.localScale = Vector3.one * 0.5f;
        Destroy(hitEffect.GetComponent<Collider>());
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = new Color(0.2f, 1f, 0.2f, 0.7f);
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", new Color(0.2f, 1f, 0.2f) * 2f);
        hitEffect.GetComponent<Renderer>().material = mat;
        Destroy(hitEffect, 0.2f);
    }

    private IEnumerator MoveParticle(GameObject particle, Vector3 startPos, Vector3 direction, float speed, float lifetime)
    {
        if (particle == null) yield break;

        float elapsed = 0f;

        while (elapsed < lifetime && particle != null)
        {
            if (particle != null)
            {
                particle.transform.position += direction * speed * Time.deltaTime;
                float scale = 0.15f * (1f - elapsed / lifetime);
                particle.transform.localScale = Vector3.one * scale;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (particle != null)
        {
            Destroy(particle);
        }
    }
}
