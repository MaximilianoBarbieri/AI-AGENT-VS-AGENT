using static Utils;

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
        if (_npc.Health < 100)
            _npc.Health += NPC_REGENERATION_LIFE;
        else
            _npc.stateMachine.ChangeState(NPCState.Await);
    }

    public override void OnExit()
    {
        _npc.Health = 100;
        _npc.SetTargetNode(_npc.leader.GetCurrentNode(), _npc.leader);
    }
}