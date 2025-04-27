using UnityEngine;

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
        Debug.Log("AWAIT STATE");
        
        if (Vector3.Distance(_npc.transform.position, _npc.leaderPos.position) > _npc.minDistanceLeader)
            _npc.stateMachine.ChangeState(NPCState.Walk);
    }

    public override void OnExit()
    {
    }
}