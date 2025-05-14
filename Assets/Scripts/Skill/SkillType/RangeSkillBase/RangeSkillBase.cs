using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeSkillBase : MonoBehaviour
{
    [SerializeField] protected Skill skillData; // 참조할 Skill SO
    [SerializeField] protected float projectileSpeed = 20f;
    [SerializeField] protected float attackDelay = 0.5f;
    [SerializeField] public LayerMask targetLayer;

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

        // 방향 정규화
        direction = direction.normalized;

        // 발사 위치 계산 (플레이어 위치에서 약간 앞으로)
        Vector3 spawnPosition = player.transform.position + direction * 1.0f;
        spawnPosition.y += 1.0f; // 바닥보다 약간 위에서 발사

        // 발사체 생성
        if (skillData.projectilePrefabs != null)
        {
            GameObject projectile = Instantiate(skillData.projectilePrefabs, spawnPosition, Quaternion.LookRotation(direction));
            SkillProjectile projectileScript = projectile.GetComponent<SkillProjectile>();

            float SkillRate = (float)((float)skillData.value / (float)100);
            // 기본 데미지
            int ArrowDamage = (int)(player._playerStat.GetStatValue(PlayerStatType.Attack) * SkillRate);


            if (projectileScript != null)
            {
                // 공통 속성 설정
                projectileScript.Init(direction, (int)projectileSpeed, player);
                projectileScript.damage = ArrowDamage;

                // 발사체 타입에 따른 속성 설정
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

        yield return new WaitForSeconds(attackDelay);
        isAttacking = false;
    }

    protected virtual void ConfigureProjectileByType(SkillProjectile projectile, ProjectileType type)
    {
        switch (type)
        {
            case ProjectileType.Penetrating:
                projectile.SetPenetrating(true, 3);
                break;

            case ProjectileType.Homing:
                projectile.SetHoming(true);
                break;

            case ProjectileType.Explosive:
                projectile.SetSplashDamage(true);
                break;

            case ProjectileType.Chain:
                // 체인 속성 설정 - 구현 필요
                break;

            case ProjectileType.Normal:
            default:
                projectile.ConfigureProjectile(false, 0, false, false);
                break;
        }
    }
    public void SetSkillData(Skill skill)
    {
        skillData = skill;
    }

    // 타겟 레이어 설정 메서드
    public void SetTargetLayer(LayerMask layer)
    {
        targetLayer = layer;
    }
}