using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCombo : MeleeSkillBase
{
    [SerializeField] private float criticalChanceBonus = 10f; // 크리티컬 확률 보너스
    [SerializeField] private float criticalDamageBonus = 40f; // 크리티컬 데미지 보너스
    [SerializeField] private float comboInterval = 0.2f; // 참격 간격
    [SerializeField] private int comboCount = 5; // 참격 횟수
    [SerializeField] private float comboDamageRate = 40f; // 참격 데미지 %

    private BoxCollider swordCollider;
    private Vector3 originalColliderSize;
    private Vector3 originalColliderCenter;

    protected void Start()
    {
        // 플레이어의 검 콜라이더 찾기
        swordCollider = GetComponentInChildren<BoxCollider>();
        if (swordCollider != null)
        {
            originalColliderSize = swordCollider.size;
            originalColliderCenter = swordCollider.center;
        }
    }

    protected override IEnumerator AttackCoroutine(Player player, Vector3 direction)
    {
        isAttacking = true;
        skillData.cooldown = skillData.maxCooldown;

        // 검 콜라이더 크기 2배로 확장
        if (swordCollider != null)
        {
            swordCollider.size = originalColliderSize * 2f;
            swordCollider.center = originalColliderCenter * 2f;
        }

        for (int i = 0; i < comboCount; i++)
        {
            // 데미지 계산
            float SkillRate = (float)((float)skillData.value / (float)100);
            int comboDamage = (int)(player._playerStat.GetStatValue(PlayerStatType.Attack) * SkillRate);

            // 크리티컬 적용
            bool isCritical = false;
            if (Random.value <= (player._playerStat.GetStatValue(PlayerStatType.CriticalChance) / 100f))
            {
                float critMultiplier = Mathf.Max(1.5f, player._playerStat.GetStatValue(PlayerStatType.CriticalDamage) / 100f);
                comboDamage = Mathf.RoundToInt(comboDamage * critMultiplier);
                isCritical = true;
            }

            // 콜라이더 활성화
            if (swordCollider != null)
            {
                swordCollider.enabled = true;
                // 데미지 처리 컴포넌트 추가
                var damageHandler = swordCollider.gameObject.AddComponent<FireComboHitBox>();
                damageHandler.Init(comboDamage, isCritical, new HashSet<Enemy>());
            }

            yield return new WaitForSeconds(comboInterval);

            // 콜라이더 비활성화
            if (swordCollider != null)
            {
                swordCollider.enabled = false;
                // 데미지 처리 컴포넌트 제거
                var damageHandler = swordCollider.gameObject.GetComponent<FireComboHitBox>();
                if (damageHandler != null)
                {
                    Destroy(damageHandler);
                }
            }
        }

        // 검 콜라이더 원래 크기로 복구
        if (swordCollider != null)
        {
            swordCollider.size = originalColliderSize;
            swordCollider.center = originalColliderCenter;
        }

        isAttacking = false;
    }

    // 크리티컬 보너스 적용/제거 메서드
    private void ApplyCriticalBonusToPlayer(Player player, bool apply)
    {
        if (player == null || player._playerStat == null) return;

        if (apply)
        {
            player._playerStat.ModifyStat(PlayerStatType.CriticalChance, criticalChanceBonus);
            player._playerStat.ModifyStat(PlayerStatType.CriticalDamage, criticalDamageBonus);
        }
        else
        {
            player._playerStat.ModifyStat(PlayerStatType.CriticalChance, -criticalChanceBonus);
            player._playerStat.ModifyStat(PlayerStatType.CriticalDamage, -criticalDamageBonus);
        }
    }
}
