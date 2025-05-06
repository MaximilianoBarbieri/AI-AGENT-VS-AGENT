using UnityEngine;
using static Utils;

public class Walk_NPC : State
{
    private NPC _npc;
    private bool _inVisionRange;

    public Walk_NPC(NPC npc)
    {
        _npc = npc;
    }

    public override void OnEnter()
    {
    }

    public override void OnUpdate()
    {
        if (_npc.HasLineOfSight())
            _npc.Flocking();
        else
            _npc.MoveAlongPath(_npc.MoveSpeed);

        if (Vector3.Distance(_npc.transform.position, _npc.leaderPos.position) <= _npc.minDistanceLeader)
            _npc.stateMachine.ChangeState(NPCState.Await);

        if (_npc.PatrolWithFoV() != null)
            _npc.stateMachine.ChangeState(NPCState.Attack);

        if (_npc.Health <= 25)
            _npc.stateMachine.ChangeState(NPCState.Escape);
    }

    public override void OnExit()
    {
    }
}