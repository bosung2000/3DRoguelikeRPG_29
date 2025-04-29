using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneArrow : RangeSkillBase
{
    [SerializeField] private float arrowSpeed = 25f;
    [SerializeField] private float arrowLifetime = 5f;
    [SerializeField] private float damageMultiplier = 1.2f;
    [SerializeField] private float criticalChanceBonus = 90f; // 크리티컬 확률 보너스
    [SerializeField] private float criticalDamageBonus = 40f; // 크리티컬 데미지 보너스

    public override void Execute(Player player, Vector3 direction)
    {
        if (isAttacking || skillData.cooldown > 0) 
            return;

        StartCoroutine(FireArrowCoroutine(player, direction));
    }

    protected virtual IEnumerator FireArrowCoroutine(Player player, Vector3 direction)
    {
        isAttacking = true;
        skillData.cooldown = skillData.maxCooldown;
        player.GetComponent<PlayerController>().SetTrigger("OneArrow");
        yield return new WaitForSeconds(0.8f);

        // 크리티컬 보너스 적용
        ApplyCriticalBonusToPlayer(player, true);

        // 방향 정규화
        direction = direction.normalized;

        // 발사 위치 계산 (플레이어 위치에서 약간 앞으로, 약간 위로)
        Vector3 spawnPosition = player.transform.position + direction * 1.2f;
        spawnPosition.y += 1.2f; // 바닥보다 높게 발사

        // 발사체 생성
        if (skillData.projectilePrefabs != null)
        {
            GameObject projectile = Instantiate(skillData.projectilePrefabs, spawnPosition, Quaternion.LookRotation(direction));
            SkillProjectile projectileScript = projectile.GetComponent<SkillProjectile>();

            if (projectileScript != null)
            {
                // 속성 설정
                projectileScript.Init(direction, (int)arrowSpeed, player);
                
                // 데미지 설정 (크리티컬은 SkillProjectile에서 처리)
                projectileScript.damage = Mathf.RoundToInt(skillData.value * damageMultiplier);
                
                // 발사체에 제한 시간 설정
                Destroy(projectile, arrowLifetime);
                
                // 발사체 타입에 따른 설정
                ConfigureProjectileByType(projectileScript, skillData.projectileType);
            }
        }

        // 발사 효과음 재생
        if (skillData.soundEffectPrefab != null)
        {
            AudioSource.PlayClipAtPoint(skillData.soundEffectPrefab, spawnPosition);
        }

        // 발사 이펙트 생성
        if (skillData.effectPrefab != null)
        {
            GameObject effect = Instantiate(skillData.effectPrefab, spawnPosition, Quaternion.LookRotation(direction));
            Destroy(effect, 2f);
        }

        // 공격 딜레이
        yield return new WaitForSeconds(attackDelay);
        
        // 크리티컬 보너스 제거
        ApplyCriticalBonusToPlayer(player, false);
        
        isAttacking = false;
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
}
