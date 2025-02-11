public class Recovery_NPC : State
{
    private NPC _npc;

    public Recovery_NPC(NPC npc)
    {
        _npc = npc;
    }

    public override void OnEnter()
    {
    }

    public override void OnUpdate()
    {
        if (_npc.Health <= 25)
            _npc.Health += NPC.RegenerationLife;
        else
            _npc.stateMachine.ChangeState(NPCState.Walk);
    }

    public override void OnExit()
    {
    }
}