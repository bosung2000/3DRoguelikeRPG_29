using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeSkillBase : MonoBehaviour
{
    [SerializeField] protected Skill skillData; // 참조할 Skill SO
    [SerializeField] protected float attackRange = 2f; // 공격 범위
    [SerializeField] protected float attackAngle = 90f; // 공격 각도
    [SerializeField] protected float attackDelay = 0.5f; // 공격 딜레이
    [SerializeField] public LayerMask targetLayer; // 공격 대상 레이어

    protected bool isAttacking = false;

    public virtual void Execute(Player player, Vector3 direction)
    {
        if (isAttacking || skillData.cooldown > 0) return;

        StartCoroutine(AttackCoroutine(player, direction));
    }

    protected virtual IEnumerator AttackCoroutine(Player player, Vector3 direction)
    {
        isAttacking = true;
        skillData.cooldown = skillData.maxCooldown;

        // 공격 방향 정규화
        direction = direction.normalized;

        // 공격 시작 위치 (플레이어 앞, 약간 위로)
        Vector3 attackStartPos = player.transform.position + Vector3.up * 0.5f;

        // 공격 범위 내의 적 탐색
        Collider[] hitColliders = Physics.OverlapSphere(attackStartPos, attackRange, targetLayer);
        
        foreach (var hitCollider in hitColliders)
        {
            float SkillRate = (float)((float)skillData.value / (float)100);
            // 기본 데미지
            int MeleeDamage = (int)(player._playerStat.GetStatValue(PlayerStatType.Attack) * SkillRate);

            // 공격 각도 내의 적만 처리
            Vector3 dirToTarget = (hitCollider.transform.position - attackStartPos).normalized;
            float angle = Vector3.Angle(direction, dirToTarget);
            
            if (angle <= attackAngle * 0.5f)
            {
                // 적에게 데미지 적용
                Enemy enemy = hitCollider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(MeleeDamage, false);
                    ShowHitEffect(hitCollider.transform.position);
                }
            }
        }

        // 공격 이펙트 표시
        ShowEffect(attackStartPos, direction, attackRange);

        yield return new WaitForSeconds(attackDelay);
        isAttacking = false;
    }

    protected virtual void ShowEffect(Vector3 center, Vector3 direction, float range)
    {
        if (skillData.effectPrefab != null)
        {
            GameObject effect = Instantiate(skillData.effectPrefab, center, Quaternion.LookRotation(direction));
            Destroy(effect, 0.5f);
        }
    }

    protected void ShowHitEffect(Vector3 position)
    {
        GameObject hitEffect = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        hitEffect.transform.position = position;
        hitEffect.transform.localScale = Vector3.one * 0.3f;

        // 콜라이더 제거
        Destroy(hitEffect.GetComponent<Collider>());

        // 재질 설정
        Material material = new Material(Shader.Find("Standard"));
        material.color = new Color(1f, 0.1f, 0.1f, 0.8f);
        material.EnableKeyword("_EMISSION");
        material.SetColor("_EmissionColor", Color.red * 2f);
        hitEffect.GetComponent<Renderer>().material = material;

        // 크기 변화 효과
        StartCoroutine(PulseEffect(hitEffect));

        // 잠시 후 제거
        Destroy(hitEffect, 0.3f);
    }

    protected IEnumerator PulseEffect(GameObject obj)
    {
        float duration = 0.3f;
        float elapsed = 0f;
        Vector3 originalScale = obj.transform.localScale;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float scale = 1f + 0.5f * Mathf.Sin(t * Mathf.PI);
            obj.transform.localScale = originalScale * scale;
            yield return null;
        }
    }

    public void SetSkillData(Skill skill)
    {
        skillData =skill;
    }

    // 타겟 레이어 설정 메서드
    public void SetTargetLayer(LayerMask layer)
    {
        targetLayer = layer;
    }

}
