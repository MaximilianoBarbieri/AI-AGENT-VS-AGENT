using static Utils;
using UnityEngine;

public class Await_Leader : State
{
    private Leader _leader;

    public Await_Leader(Leader leader)
    {
        _leader = leader;
    }

    public override void OnEnter()
    {
    }

    public override void OnUpdate()
    {
        if (_leader.useTheta || _leader.useMove)
            _leader.stateMachine.ChangeState(LeaderState.Walk);
    }

    public override void OnExit()
    {
    }
}