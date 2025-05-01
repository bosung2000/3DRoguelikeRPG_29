using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPhaseState : IBossState
{
    public void EnterState(BossController controller)
    {
        controller.GetComponent<Boss>().IsPhase2 = true;
        controller.ChageState(BossStateType.KeepDistance);
    }

    public void ExitState(BossController controller)
    {

    }

    public void UpdateState(BossController controller)
    {

    }
}
