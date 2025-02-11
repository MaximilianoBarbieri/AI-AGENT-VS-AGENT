using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_NPC : State
{
    private NPC _npc;

    public Attack_NPC(NPC npc)
    {
        _npc = npc;
    }

    public override void OnEnter()
    {
    }

    public override void OnUpdate()
    {
        if (_npc.Health <= 25)
            _npc.stateMachine.ChangeState(NPCState.Escape);
    }

    public override void OnExit()
    {
    }
}