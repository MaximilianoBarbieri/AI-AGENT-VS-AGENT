using UnityEngine;
using static Utils;

public class Walk_Leader : State
{
    private Leader _leader;

    public Walk_Leader(Leader leader)
    {
        _leader = leader;
    }


    public override void OnEnter()
    {
    }

    public override void OnUpdate()
    {
        if (_leader.useMove)
            _leader.Move();
        else if (_leader.useTheta && _leader.targetNode != null)
            _leader.MoveAlongPath(LEADER_MOVE_SPEED);

        if (Vector3.Distance(_leader.transform.position, _leader.directTargetPos) <= 0.1f)
            stateMachine.ChangeState(LeaderState.Await);
    }

    public override void OnExit()
    {
        _leader.useMove = false;
        _leader.useTheta = false;
    }
}