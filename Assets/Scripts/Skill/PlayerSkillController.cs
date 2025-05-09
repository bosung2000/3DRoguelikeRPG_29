using UnityEngine;

public class PlayerSkillController : MonoBehaviour
{
    [SerializeField] private GameObject[] slashEffects;
    [SerializeField] private Transform effectSpawnPoint;
    private Animator animator;

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

    public void UseSlashSkill(int effectIndex)
    {
        if (effectIndex < 0 || effectIndex >= slashEffects.Length) return;

        // 애니메이션 재생
        animator.SetTrigger("Attack");

        // 이펙트 생성
        GameObject effect = Instantiate(slashEffects[effectIndex], effectSpawnPoint.position, effectSpawnPoint.rotation);

        // 프로젝타일인 경우
        if (effect.TryGetComponent(out MaykerStudio.Demo.Projectile projectile))
        {
            projectile.Fire();
        }
        // 일반 파티클인 경우
        else if (effect.TryGetComponent(out ParticleSystem particleSystem))
        {
            particleSystem.Play(true);
        }
    }
}