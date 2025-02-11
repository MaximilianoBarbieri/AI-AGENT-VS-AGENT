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
    }

    public override void OnUpdate()
    {
        //if(_npc.isSafe)
        //_npc.stateMachine.ChangeState(NPCState.Recovery)
    }

    public override void OnExit()
    {
    }
}