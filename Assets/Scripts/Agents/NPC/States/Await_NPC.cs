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
        _npc.MoveSpeed = NPC_AWAIT_MOVE_SPEED;
    }

    public override void OnUpdate()
    {
        float distToLeader = Vector3.Distance(_npc.transform.position, _npc.leaderPos.position);

        if (distToLeader > _npc.minDistanceLeader)
            _npc.stateMachine.ChangeState(NPCState.Walk);
        else if (distToLeader <= _npc.minDistanceLeader)
            _npc.Flocking();

        if (_npc.PatrolWithFoV() != null)
            _npc.stateMachine.ChangeState(NPCState.Attack);

        if (_npc.Health <= 25)
            _npc.stateMachine.ChangeState(NPCState.Escape);
    }

    public override void OnExit()
    {
        _npc.MoveSpeed = NPC_ORIGINAL_MOVE_SPEED;
    }
}