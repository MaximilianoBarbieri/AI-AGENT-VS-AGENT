using UnityEngine;

public class Walk_NPC : State
{
    private NPC _npc;
    private bool _useFlocking;

    public Walk_NPC(NPC npc)
    {
        _npc = npc;
    }

    public override void OnEnter()
    {
        Debug.Log("ON ENTER DE WALK");
        _useFlocking = _npc.HasLineOfSight();

        _npc.SetTargetNode(_npc.leaderPos.position);
    }

    public override void OnUpdate()
    {
        Debug.Log("WALK STATE");

        if (!_useFlocking)
            _npc.Flocking();
        else
            _npc.MoveAlongPath();

        if (Vector3.Distance(_npc.transform.position, _npc.leaderPos.position) <= _npc.minDistanceLeader)
            _npc.stateMachine.ChangeState(NPCState.Await);
    }

    public override void OnExit()
    {
    }
}