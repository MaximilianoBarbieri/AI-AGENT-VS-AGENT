using System.Collections.Generic;
using UnityEngine;
using static Utils;

public class NPC : Entity
{
    public float Health { get; set; } = 100f;
    public Vector3 Velocity { get; private set; }
    public List<NPC> Neighbors { get; private set; } = new();

    public float MoveSpeed { get; set; } = NPC_ORIGINAL_MOVE_SPEED;

    public NPC currentEnemy;

    [Header("Flocking Properties")] private List<IMovementBehaviour> _behaviors;

    public Transform leaderPos;
    public Leader leader;

    [Range(0, 5)] public float cohesionWeight = 1.0f;
    [Range(0, 5)] public float alignmentWeight = 1.0f;
    [Range(0, 5)] public float separationWeight = 1.5f;
    [Range(0, 5)] public float separationDistance = 2.0f;
    [Range(0, 5)] public float leaderFollowWeight = 1.5f;
    [Range(0, 5)] public float minDistanceLeader = 1f;
    [Range(0, 5)] public float avoidWeight = 1.5f;

    public LineRenderer attackFXRenderer;
    public StateMachine stateMachine;

    private void Start()
    {
        leaderPos = leader.transform;

        attackFXRenderer = GetComponent<LineRenderer>();

        stateMachine = gameObject.AddComponent<StateMachine>();

        stateMachine.AddState(NPCState.Await, new Await_NPC(this));
        stateMachine.AddState(NPCState.Walk, new Walk_NPC(this));
        stateMachine.AddState(NPCState.Attack, new Attack_NPC(this));
        stateMachine.AddState(NPCState.Escape, new Escape_NPC(this));
        stateMachine.AddState(NPCState.Recovery, new Recovery_NPC(this));
        stateMachine.AddState(NPCState.Death, new Death_NPC(this));

        stateMachine.ChangeState(NPCState.Await);

        _behaviors = new List<IMovementBehaviour>
        {
            new CohesionBehaviour(),
            new AlignmentBehavior(),
            new SeparationBehavior(),
            new LeaderFollowingBehavior(),
            new ObstacleAvoidanceBehavior()
        };
    }

    private void Update() => DetectNeighbors();

    public void Flocking()
    {
        Vector3 movement = Vector3.zero;

        Vector3 obstacleAvoidance = _behaviors
            .Find(behavior => behavior is ObstacleAvoidanceBehavior)
            ?.CalculateSteeringVelocity(this) ?? Vector3.zero;

        if (obstacleAvoidance.magnitude > 0.1f)
        {
            movement = obstacleAvoidance;
        }
        else
        {
            foreach (var behavior in _behaviors)
            {
                if (behavior is ObstacleAvoidanceBehavior) continue;

                movement += behavior.CalculateSteeringVelocity(this);
            }
        }

        Velocity = movement.normalized * MoveSpeed;
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

        foreach (var col in Physics.OverlapSphere(transform.position, NPC_VIEWRADIUS))
        {
            NPC npc = col.GetComponent<NPC>();

            if (npc == null || npc.leader.myTeam == leader.myTeam)
                continue;

            Vector3 dir = (npc.transform.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, dir) <= NPC_VIEWANGLE / 2f)
            {
                float dist = Vector3.Distance(transform.position, npc.transform.position);

                if (Physics.Raycast(transform.position, dir, out RaycastHit hit, NPC_VIEWRADIUS, layerMask))
                    if (hit.collider.gameObject != npc.gameObject)
                        continue;

                if (dist < minDist)
                {
                    minDist = dist;
                    closestEnemy = npc;
                }
            }
        }

        currentEnemy = closestEnemy;

        if (currentEnemy != null)
            Debug.Log("¡Enemigo más cercano en el FoV: " + currentEnemy.name + "!");

        return currentEnemy;
    }

    public override bool HasLineOfSight()
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 directionToTarget = (leaderPos.position - transform.position).normalized;
        float maxDistance = Vector3.Distance(transform.position, leaderPos.position);

        return !Physics.Raycast(origin, directionToTarget.normalized, maxDistance, _obstacleMask);
    }

    public override string ObstacleAvoidance()
    {
        Vector3 leftRayOrigin = transform.position + transform.right * -0.5f;
        Vector3 rightRayOrigin = transform.position + transform.right * 0.5f;

        bool leftHit = Physics.Raycast(leftRayOrigin, transform.forward, NPC_DISTANCE_OBSTACLE_AVOIDANCE,
            _obstacleMask);
        bool rightHit = Physics.Raycast(rightRayOrigin, transform.forward, NPC_DISTANCE_OBSTACLE_AVOIDANCE,
            _obstacleMask);

        return leftHit ? "Left" : rightHit ? "Right" : "None";
    }

    public override void TakeDamage(int dmg)
    {
        Health -= dmg;

        if (Health <= 0)
            Destroy(gameObject);
    }

    public override bool ShouldReactToLeader(Leader clickedLeader)
    {
        return clickedLeader == leader;
    }

    private void OnEnable()
    {
        OnSetTargetNode += SetTargetNode;
    }

    private void OnDisable()
    {
        OnSetTargetNode -= SetTargetNode;
    }

    private void OnDrawGizmos()
    {
        if (leaderPos != null)
        {
            Vector3 origin = transform.position + Vector3.up * 0.5f;
            Vector3 directionToTarget = (leaderPos.position - transform.position).normalized;
            float maxDistance = Vector3.Distance(transform.position, leaderPos.position);

            bool hasLOS = !Physics.Raycast(origin, directionToTarget.normalized, maxDistance, _obstacleMask);

            Gizmos.color = hasLOS ? Color.green : Color.red;
            Gizmos.DrawLine(transform.position, leaderPos.position);
        }
    }
}