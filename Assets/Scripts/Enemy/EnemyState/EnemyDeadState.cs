using UnityEngine;

public class EnemyDeadState : IEnemyState
{
    Enemy enemy;

    public void EnterState(EnemyController controller)
    {
        enemy = controller.Enemy;
        controller.agent.isStopped = true;
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