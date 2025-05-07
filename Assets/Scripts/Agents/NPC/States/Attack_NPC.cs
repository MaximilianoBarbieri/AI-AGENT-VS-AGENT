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
        _npc.attackFXRenderer.SetPosition(1, _npc.CurrentEnemy.transform.position);

        _npc.CurrentEnemy.TakeDamage(NPC_DAMAGE);
    }

    public override void OnUpdate()
    {
        if (_npc.Health <= NPC_MIN_HEALTH_TO_RECOVERY)
        {
            _npc.stateMachine.ChangeState(NPCState.Escape);
            return;
        }

        _currentCd += Time.deltaTime;

        if (_npc.CurrentEnemy != null)
            _npc.transform.LookAt(_npc.CurrentEnemy.transform);

        if (_currentCd >= NPC_CD_ATTACK_ORIG)
            _npc.stateMachine.ChangeState(NPCState.Await);
    }

    public override void OnExit()
    {
        _npc.attackFXRenderer.enabled = false;
        _currentCd = 0;
    }
}