using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death_NPC : State
{
    private NPC _npc;

    public Death_NPC(NPC npc)
    {
        _npc = npc;
    }

    public override void OnEnter()
    {
    }

    public override void OnUpdate()
    {
    }

    public override void OnExit()
    {
    }
}