using UnityEngine;

public class EnemyHitState : IEnemyState
{
    Enemy enemy;
    private float _hitDuration = 0.3f; // 경직 시간
    private float _timer = 0f;
    public void EnterState(EnemyController controller)
    {
        enemy = controller.Enemy;
        if(enemy.IsBoss && controller.GetCurrentHP() > 0)
        {
            return;
        }

        _timer = 0f;
        controller.agent.isStopped = true;
        controller.agent.ResetPath();
        controller.animator.SetTrigger("Hit");
        controller.ResetAttackCooldown();
    }

    public void ExitState(EnemyController controller)
    {

    }

    public void UpdateState(EnemyController controller)
    {
        _timer += Time.deltaTime;
        if (_timer > _hitDuration)
        {
            if (controller.GetCurrentHP() <= 0)
            {
                controller.ChageState(EnemyStateType.Dead);
            }
            else
            {
                switch(controller.LastStateType)
                {
                    case EnemyStateType.Chase:
                        controller.ChageState(EnemyStateType.Chase);
                        break;
                    default:
                        controller.ChageState(EnemyStateType.Idle);
                        break;
                }
            }
        }
    }
}