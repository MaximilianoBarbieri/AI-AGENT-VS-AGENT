using UnityEditor.VersionControl;

public class Walk_NPC : State
{
    private NPC _npc;

    public Walk_NPC(NPC npc)
    {
        _npc = npc;
    }

    public override void OnEnter()
    {
    }

    public override void OnUpdate()
    {
//        if (_npc.Health <= 25)
//            _npc.stateMachine.ChangeState(NPCState.Escape);

        if (_npc.isFlocking)
            _npc.Flocking();
        else
            _npc.MoveAlongPath();
    }

    public override void OnExit()
    {
    }
}