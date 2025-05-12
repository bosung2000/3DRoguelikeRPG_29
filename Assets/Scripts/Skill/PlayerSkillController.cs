using UnityEngine;
using System.Collections.Generic;

public class PlayerSkillController : MonoBehaviour
{
    [Header("이펙트 설정")]
    [SerializeField] private GameObject[] slashEffects; // OrdosFX 프리팹
    [SerializeField] private Transform effectSpawnPoint;
    
    [Header("현재 활성화된 이펙트")]
    private List<GameObject> activeEffects = new List<GameObject>();
    
    [Header("애니메이션")]
    private Animator animator;
    [SerializeField] private float effectDuration = 2f; // 이펙트 지속 시간

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // 공격 입력 감지
        //if (Input.GetMouseButtonDown(0))
        //{
        //    UseSlashSkill(0); // 첫번째 이펙트 사용
        //}
        //else if (Input.GetMouseButtonDown(1))
        //{
        //    UseSlashSkill(1); // 두번째 이펙트 사용
        //}
    }

    // 이 메서드는 외부에서 호출하거나 버튼 이벤트로 연결할 수 있습니다
    public void UseSlashSkill(int effectIndex)
    {
        if (effectIndex < 0 || effectIndex >= slashEffects.Length) return;

        // 애니메이션 재생
        //animator.SetTrigger("Attack");

        // 기존 이펙트 모두 제거
        CleanupActiveEffects();

        // 새 이펙트 생성
        GameObject effect = Instantiate(slashEffects[effectIndex], effectSpawnPoint.position, effectSpawnPoint.rotation);
        
        // 플레이어 이동에 따라 이펙트도 같이 이동하도록 부모 설정
        effect.transform.SetParent(effectSpawnPoint);
        effect.transform.localPosition = Vector3.zero;
        effect.transform.localRotation = Quaternion.identity;
        
        // 활성화된 이펙트 목록에 추가
        activeEffects.Add(effect);
        
        //// 프로젝타일인 경우
        //if (effect.TryGetComponent(out MaykerStudio.Demo.Projectile projectile))
        //{
        //    projectile.Fire();
        //    // 부모 해제 (독립적으로 이동하기 위해)
        //    effect.transform.SetParent(null);
        //}
        //// 일반 파티클인 경우
        //else if (effect.TryGetComponent(out ParticleSystem particleSystem))
        //{
        //    particleSystem.Play(true);
        //}
        
        // 일정 시간 후 자동 제거
        Destroy(effect, effectDuration);
    }
    
    // 애니메이션 이벤트에서 호출할 수 있는 메서드
    public void PlayEffectFromAnimation(int effectIndex)
    {
        UseSlashSkill(effectIndex);
    }
    
    // 기존 활성화된 이펙트 모두 제거
    private void CleanupActiveEffects()
    {
        foreach (GameObject effect in activeEffects)
        {
            if (effect != null)
            {
                Destroy(effect);
            }
        }
        
        activeEffects.Clear();
    }
}