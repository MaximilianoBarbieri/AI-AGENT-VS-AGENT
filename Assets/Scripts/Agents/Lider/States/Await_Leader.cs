using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Await_Leader : State
{
    private Lider _lider;

    public Await_Leader(Lider lider)
    {
        _lider = lider;
    }

    public override void OnEnter()
    {
    }

    public override void OnUpdate()
    {
        if (Input.GetMouseButtonDown((int)_lider._buttonDown))
            _lider.SetTargetNode();

    }

    public override void OnExit()
    {
    }
}