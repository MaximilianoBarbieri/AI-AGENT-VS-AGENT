using static Utils;
using UnityEngine;

public class Attack_Leader : State
{
    private Leader _leader;
    private float _currentCd = 0;

    public Attack_Leader(Leader leader)
    {
        _leader = leader;
    }

    public override void OnEnter()
    {
        Attack();
    }

    public override void OnUpdate()
    {
        _currentCd += Time.deltaTime;

        if (_currentCd >= LEADER_CD_ATTACK_ORIG)
            _leader.stateMachine.ChangeState(LeaderState.Await);
    }

    public override void OnExit()
    {
        _currentCd = 0;
    }

    private void Attack()
    {
        Collider[] hits = Physics.OverlapSphere(_leader.transform.position, LEADER_RADIUS_ATTACK);

        foreach (var hit in hits)
        {
            NPC npc = hit.GetComponent<NPC>();

            if (npc != null && npc.leader != null && npc.leader.myTeam != _leader.myTeam)
            {
                npc.TakeDamage(LEADER_DAMAGE);
                Debug.Log(npc.name + " ACABA DE RECIBIR DAMAGE");
            }
            
        }
    }
}