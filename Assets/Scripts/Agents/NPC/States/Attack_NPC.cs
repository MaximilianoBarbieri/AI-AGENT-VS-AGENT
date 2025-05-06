using UnityEngine;
using static Utils;

public class Attack_NPC : State
{
    private NPC _npc;
    private float _currentCd = 0;

    public Attack_NPC(NPC npc)
    {
        _npc = npc;
    }

    public override void OnEnter()
    {
        _npc.attackFXRenderer.enabled = true;

        _npc.attackFXRenderer.SetPosition(0, _npc.transform.position);
        _npc.attackFXRenderer.SetPosition(1, _npc.currentEnemy.transform.position);

        _npc.currentEnemy.TakeDamage(NPC_DAMAGE);
    }

    public override void OnUpdate()
    {
        _currentCd += Time.deltaTime;

        if (_currentCd >= NPC_CD_ATTACK_ORIG)
            _npc.stateMachine.ChangeState(NPCState.Await);

        if (_npc.Health <= 25)
            _npc.stateMachine.ChangeState(NPCState.Escape);
    }

    public override void OnExit()
    {
        _npc.attackFXRenderer.enabled = false;
        _currentCd = 0;
    }
}