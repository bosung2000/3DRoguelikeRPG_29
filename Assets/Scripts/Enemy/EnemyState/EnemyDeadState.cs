using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeadState : IEnemyState
{
    private float _deadDuration = 1.5f;
    private float _timer = 0f;
    private bool _animationEnded = false;

    public void EnterState(EnemyController controller)
    {
        _timer = 0f;
        controller.agent.isStopped = true; ;

        controller.animator.SetTrigger("DIe");
    }

    public void ExitState(EnemyController controller)
    {

    }

    public void UpdateState(EnemyController controller)
    {
        Enemy enemy = controller.Enemy;

        if(enemy == null ) return;

        if(!enemy.IsDeadAnimationEnded())
        {
            return;
        }

        _timer += Time.deltaTime;

        if( _timer >= _deadDuration )
        {
            GameObject.Destroy(controller.gameObject);
        }
    }
}
