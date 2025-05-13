using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCombo : MeleeSkillBase
{
    [SerializeField] private float criticalChanceBonus = 10f; // 크리티컬 확률 보너스
    [SerializeField] private float criticalDamageBonus = 40f; // 크리티컬 데미지 보너스
    [SerializeField] private float comboInterval = 0.2f; // 참격 간격
    [SerializeField] private float hitBoxLength = 2.0f; // 콜라이더 길이(검의 2배)
    [SerializeField] private float hitBoxWidth = 0.3f; // 콜라이더 폭
    [SerializeField] private float hitBoxHeight = 0.3f; // 콜라이더 높이
    [SerializeField] private int comboCount = 5; // 참격 횟수
    [SerializeField] private float comboDamageRate = 40f; // 참격 데미지 %

    public override void Execute(Player player, Vector3 direction)
    {
        if (isAttacking || skillData.cooldown > 0) 
            return;
        StartCoroutine(FireComboCoroutine(player, direction));
    }

    private IEnumerator FireComboCoroutine(Player player, Vector3 direction)
    {
        isAttacking = true;
        skillData.cooldown = skillData.maxCooldown;

        for (int i = 0; i < comboCount; i++)
        {
            // 방향 정규화
            direction = direction.normalized;
            // 콜라이더 위치/회전 계산
            Vector3 hitBoxPos = player.transform.position + direction * 1.2f + Vector3.up * 1.2f;
            Quaternion hitBoxRot = Quaternion.LookRotation(direction);

            // 콜라이더 생성
            GameObject hitBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
            hitBox.transform.position = hitBoxPos;
            hitBox.transform.rotation = hitBoxRot;
            hitBox.transform.localScale = new Vector3(hitBoxWidth, hitBoxHeight, hitBoxLength);
            var collider = hitBox.GetComponent<Collider>();
            if (collider != null) collider.isTrigger = true;
            var renderer = hitBox.GetComponent<Renderer>();
            if (renderer != null) Destroy(renderer);

            // 데미지 계산
            float SkillRate = comboDamageRate / 100f;
            int comboDamage = (int)(player._playerStat.GetStatValue(PlayerStatType.Attack) * SkillRate);

            // 크리티컬 적용
            bool isCritical = false;
            if (Random.value <= (player._playerStat.GetStatValue(PlayerStatType.CriticalChance) / 100f))
            {
                float critMultiplier = Mathf.Max(1.5f, player._playerStat.GetStatValue(PlayerStatType.CriticalDamage) / 100f);
                comboDamage = Mathf.RoundToInt(comboDamage * critMultiplier);
                isCritical = true;
            }

            // 각 참격마다 HashSet 새로 생성
            HashSet<Enemy> alreadyHit = new HashSet<Enemy>();
            hitBox.AddComponent<FireComboHitBox>().Init(comboDamage, isCritical, alreadyHit);

            Destroy(hitBox, 0.1f);
            yield return new WaitForSeconds(comboInterval);
        }
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
