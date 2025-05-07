using System.Collections.Generic;
using UnityEngine;
using static Utils;

public class NPC : Entity
{
    public Leader leader;
    public List<NPC> Neighbors { get; private set; } = new();
    public NPC CurrentEnemy { get; private set; }
    public Transform LeaderPos { get; private set; }

    [Header("Flocking Properties")] private List<IFlockingBehaviour> _flocking;

    [Range(0, 5)] public float cohesionWeight = 1.0f;
    [Range(0, 5)] public float alignmentWeight = 1.0f;
    [Range(0, 5)] public float separationWeight = 1.5f;
    [Range(0, 5)] public float separationDistance = 2.0f;

    [Range(0, 5)] public float leaderFollowWeight = 1.5f;
    [Range(0, 5)] public float minDistanceLeader = 1f;
    
    public LineRenderer attackFXRenderer;

    private void Start()
    {
        LeaderPos = leader.transform;

        attackFXRenderer = GetComponent<LineRenderer>();

        stateMachine = gameObject.AddComponent<StateMachine>();

        stateMachine.AddState(NPCState.Await, new Await_NPC(this));
        stateMachine.AddState(NPCState.Walk, new Walk_NPC(this));
        stateMachine.AddState(NPCState.Attack, new Attack_NPC(this));
        stateMachine.AddState(NPCState.Escape, new Escape_NPC(this));
        stateMachine.AddState(NPCState.Recovery, new Recovery_NPC(this));
        stateMachine.AddState(NPCState.Death, new Death_NPC(this));

        stateMachine.ChangeState(NPCState.Await);

        _flocking = new List<IFlockingBehaviour>
        {
            new CohesionBehaviour(),
            new AlignmentBehavior(),
            new SeparationBehavior(),
            new LeaderFollowingBehavior(),
        };

        _obstacleAvoidance = new ObstacleAvoidanceBehavior();
    }

    private void Update() => DetectNeighbors();

    public void Flocking()
    {
        Vector3 movement = Vector3.zero;

        Vector3 obstacleAvoidance = _obstacleAvoidance.CalculateSteeringVelocity(this);

        if (obstacleAvoidance.magnitude > 0.1f)
            movement = obstacleAvoidance;
        else
            foreach (var behavior in _flocking)
                movement += behavior.CalculateSteeringVelocity(this);

        Velocity = new Vector3(movement.x, 0, movement.z).normalized * NPC_ORIGINAL_MOVE_SPEED;
        transform.position += Velocity * Time.deltaTime;

        if (Velocity.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(Velocity);
            transform.rotation =
                Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * NPC_ROTATION_SPEED);
        }
    }

    private void DetectNeighbors()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, separationDistance);

        HashSet<NPC> newNeighbors = new HashSet<NPC>();

        foreach (Collider col in colliders)
        {
            NPC npc = col.GetComponent<NPC>();

            if (npc != null && npc != this)
            {
                newNeighbors.Add(npc);
            }
        }

        Neighbors = new List<NPC>(newNeighbors);
    }

    public NPC PatrolWithFoV()
    {
        float minDist = Mathf.Infinity;
        NPC closestEnemy = null;
        int layerMask = ~(1 << nodeLayer);

        Collider[] hits = Physics.OverlapSphere(transform.position, NPC_VIEWRADIUS);

        foreach (var col in hits)
        {
            NPC npc = col.GetComponent<NPC>();
            if (npc == null || npc == this) continue;

            if (npc.leader.myTeam == leader.myTeam)
                continue;

            Vector3 directionToTarget = (npc.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, directionToTarget);

            if (angle > NPC_VIEWANGLE / 2f)
                continue;

            float distanceToTarget = Vector3.Distance(transform.position, npc.transform.position);

            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, directionToTarget, out RaycastHit hit,
                    distanceToTarget, layerMask))
            {
                if (hit.collider.gameObject != npc.gameObject)
                    continue;
            }

            if (distanceToTarget < minDist)
            {
                minDist = distanceToTarget;
                closestEnemy = npc;
            }
        }

        CurrentEnemy = closestEnemy;

        if (CurrentEnemy != null)
            Debug.Log("¡Enemigo más cercano en el FoV: " + CurrentEnemy.name + "!");

        return CurrentEnemy;
    }

    public override bool HasLineOfSight()
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 directionToTarget = (LeaderPos.position - transform.position).normalized;
        float maxDistance = Vector3.Distance(transform.position, LeaderPos.position);

        return !Physics.Raycast(origin, directionToTarget.normalized, maxDistance, _obstacleMask);
    }

    public void TakeDamage(int dmg)
    {
        Health -= dmg;
        
        if (Health <= 0)
            Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Vector3 pos = transform.position;
        Vector3 forward = transform.forward;

        float halfAngle = NPC_VIEWANGLE / 2f;
        Vector3 leftDir = Quaternion.Euler(0, -halfAngle, 0) * forward;
        Vector3 rightDir = Quaternion.Euler(0, halfAngle, 0) * forward;

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(pos, leftDir * NPC_VIEWRADIUS);
        Gizmos.DrawRay(pos, rightDir * NPC_VIEWRADIUS);

        // Línea al enemigo actual si existe
        if (CurrentEnemy != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(pos + Vector3.up * 0.5f, CurrentEnemy.transform.position + Vector3.up * 0.5f);
        }

        // Línea al líder, verde si tiene LoS, roja si no
        if (LeaderPos != null)
        {
            Vector3 origin = pos + Vector3.up * 0.5f;
            Vector3 dirToLeader = (LeaderPos.position - pos).normalized;
            float distToLeader = Vector3.Distance(pos, LeaderPos.position);

            Gizmos.color = HasLineOfSight() ? Color.green : Color.red;
            Gizmos.DrawRay(origin, dirToLeader * distToLeader);
        }
    }
}