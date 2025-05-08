using System.Collections.Generic;
using UnityEngine;
using static Utils;

public class NPC : Entity
{
    public Leader leader;
    public List<NPC> Neighbors { get; private set; } = new();
    public NPC CurrentEnemy { get; private set; }
    public Transform LeaderPos { get; private set; }
    
    [Header("FLOCKING PROPERTIES")] 

    [Range(0, 5)] public float cohesionWeight = 1.0f;
    [Range(0, 5)] public float alignmentWeight = 1.0f;
    [Range(0, 5)] public float separationWeight = 1.5f;
    [Range(0, 5)] public float separationDistance = 2.0f;

    [Range(0, 5)] public float leaderFollowWeight = 1.5f;
    [Range(0, 5)] public float minDistanceLeader = 1f;
   
    private List<IFlockingBehaviour> _flocking;

    [Header("FX")]
    
    public LineRenderer attackFXRenderer;

    public NPC()
    {
        MaxSpeed = NPC_ORIGINAL_MOVE_SPEED;
        ViewRadius = NPC_VIEWRADIUS;
    }

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

    public override void Move()
    {
        Vector3 steering = Vector3.zero;

        Vector3 obstacleAvoidance = _obstacleAvoidance.CalculateSteeringVelocity(this);
        
        if (obstacleAvoidance.magnitude > 0.1f)
            steering = obstacleAvoidance;
        else
        {
            foreach (var behavior in _flocking)
                steering += behavior.CalculateSteeringVelocity(this);

            Vector3 arriveToLeader = Arrive(leader.transform.position);
            steering += arriveToLeader;
        }

        Velocity += steering;
        Velocity = Vector3.ClampMagnitude(Velocity, MaxSpeed);
        
        transform.position += Velocity * Time.deltaTime;

        if (Velocity.sqrMagnitude > 0.01f)
            transform.forward = Velocity.normalized;
    }

    private void DetectNeighbors()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, separationDistance);

        HashSet<NPC> newNeighbors = new HashSet<NPC>();

        foreach (Collider col in colliders)
        {
            NPC npc = col.GetComponent<NPC>();

            if (npc != null && npc != this)
                newNeighbors.Add(npc);
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

    public void TakeDamage(int dmg)
    {
        Health -= dmg;

        if (Health <= 0)
            Destroy(gameObject);
    }
}