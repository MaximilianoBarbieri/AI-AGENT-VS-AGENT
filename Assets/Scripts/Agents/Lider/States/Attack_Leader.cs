using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Leader : State
{
    private Lider _lider;

    public Attack_Leader(Lider lider)
    {
        _lider = lider;
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