using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walk_Leader : State
{
    private Lider _lider;

    public Walk_Leader(Lider lider)
    {
        _lider = lider;
    }


    public override void OnEnter()
    {
    }

    public override void OnUpdate()
    {
        _lider.MoveAlongPath();
    }

    public override void OnExit()
    {
    }
}