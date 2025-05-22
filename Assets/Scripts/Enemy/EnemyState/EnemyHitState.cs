using UnityEngine;

public class EnemyHitState : IEnemyState
{
    Enemy enemy;
    private float _hitDuration = 0.3f; // 경직 시간
    private float _timer = 0f;
    public void EnterState(EnemyController controller)
    {
        enemy = controller.Enemy;
        _timer = 0f;
        controller.agent.isStopped = true;
        controller.agent.ResetPath();
        controller.animator.ResetTrigger("Attack");
        controller.animator.SetTrigger("Hit");
        controller.animator.SetBool("isHit", true);
        controller.ResetAttackCooldown();

        Debug.Log(controller.LastStateType);
    }

    public void ExitState(EnemyController controller)
    {
        controller.animator.SetBool("isHit", false);
    }

    public void UpdateState(EnemyController controller)
    {
        controller.Enemy.DisableWeaponCollider();

        AnimatorStateInfo stateInfo = controller.animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Hit") && stateInfo.normalizedTime >= 0.9f)
        {
            if (controller.GetCurrentHP() <= 0)
            {
                controller.ChageState(EnemyStateType.Dead);
            }
                        controller.ChageState(EnemyStateType.Chase);
        }
    }
}