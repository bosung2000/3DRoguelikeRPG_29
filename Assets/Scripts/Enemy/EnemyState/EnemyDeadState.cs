using UnityEngine;

public class EnemyDeadState : IEnemyState
{
    Enemy enemy;

    public void EnterState(EnemyController controller)
    {
        enemy = controller.Enemy;
        if (!controller.agent.enabled)
        {
            controller.agent.enabled = true;
        }
        controller.agent.isStopped = true;
        controller.animator.SetBool("isRun", false);
        controller.animator.SetBool("isWalk", false);
        controller.animator.ResetTrigger("Attack");
        controller.animator.ResetTrigger("Skill");
        controller.animator.ResetTrigger("Hit");
        controller.animator.SetTrigger("DIe");
    }

    public void ExitState(EnemyController controller)
    {

    }

    public void UpdateState(EnemyController controller)
    {
        if (enemy == null) return;

        if (enemy.IsDeadAnimationEnded())
        {
            GameObject.Destroy(controller.gameObject);
        }
    }
}