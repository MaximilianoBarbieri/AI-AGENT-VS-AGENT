using UnityEngine;
using static Utils;

public class Walk_NPC : State
{
    private NPC _npc;
    private Node _currentTargetNode;

    public Walk_NPC(NPC npc)
    {
        _npc = npc;
    }

    public override void OnEnter()
    {
        _currentTargetNode = _npc.leader.GetCurrentNode();

        _npc.path = ThetaManager.FindPath(_npc.GetCurrentNode(), _currentTargetNode);
    }

    public override void OnUpdate()
    {
        if (_npc.HasLineOfSight(_npc.LeaderPos.position))
            _npc.Move();
        else
            RecalculatedPath();

        if (Vector3.Distance(_npc.transform.position, _npc.LeaderPos.position) <= _npc.minDistanceLeader)
            _npc.stateMachine.ChangeState(NPCState.Await);

        if (_npc.PatrolWithFoV() != null)
            _npc.stateMachine.ChangeState(NPCState.Attack);
    }

    public override void OnExit()
    {
    }

    private void RecalculatedPath()
    {
        if (_currentTargetNode != _npc.leader.GetCurrentNode())
        {
            _currentTargetNode = _npc.leader.GetCurrentNode();
            _npc.path = ThetaManager.FindPath(_npc.GetCurrentNode(), _currentTargetNode);
        }

        _npc.MoveAlongPath(NPC_ORIGINAL_MOVE_SPEED);
    }
}