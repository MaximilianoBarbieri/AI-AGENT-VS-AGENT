using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Leader : State
{
    private Leader _leader;

    public Attack_Leader(Leader leader)
    {
        _leader = leader;
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