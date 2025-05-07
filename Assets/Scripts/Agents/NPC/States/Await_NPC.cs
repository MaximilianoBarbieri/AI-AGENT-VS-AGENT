using UnityEngine;
using static Utils;

public class Await_NPC : State
{
    private NPC _npc;

    public Await_NPC(NPC npc)
    {
        _npc = npc;
    }

    public override void OnEnter()
    {
    }

    public override void OnUpdate()
    {
        if (_npc.Health <= NPC_MIN_HEALTH_TO_RECOVERY)
            stateMachine.ChangeState(NPCState.Escape);

        float distToLeader = Vector3.Distance(_npc.transform.position, _npc.LeaderPos.position);

        if (distToLeader > _npc.minDistanceLeader)
            _npc.stateMachine.ChangeState(NPCState.Walk);

        if (_npc.PatrolWithFoV() != null)
            _npc.stateMachine.ChangeState(NPCState.Attack);
    }

    public override void OnExit()
    {
    }
}