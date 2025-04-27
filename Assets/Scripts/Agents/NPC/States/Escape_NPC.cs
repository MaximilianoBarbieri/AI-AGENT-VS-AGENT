using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Escape_NPC : State
{
    private NPC _npc;

    public Escape_NPC(NPC npc)
    {
        _npc = npc;
    }

    public override void OnEnter()
    {
        _npc.SetTargetNode(_npc.leader.safeZone);
    }

    public override void OnUpdate()
    {
        if (Vector3.Distance(_npc.transform.position, _npc.leader.safeZone) > 0.1f)
            _npc.MoveAlongPath();
        else
            _npc.stateMachine.ChangeState(NPCState.Recovery);
    }

    public override void OnExit()
    {
    }
}