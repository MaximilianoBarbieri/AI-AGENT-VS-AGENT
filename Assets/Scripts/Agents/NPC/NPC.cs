using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class NPC : MoveNodeBase
{
    public float Health { get; set; } = 100f;
    [SerializeField] public List<NPC> Neighbors = new();
    public List<NPC> Enemys { get; private set; } = new();
    public Vector3 Velocity { get; private set; }

    private const float Damage = 5;
    private const float MoveSpeed = 4.5f;
    private const float ViewAngle = 60;

    public const float RegenerationLife = 0.5f;

    [Range(0, 10)] public float minDistanceLeader = 1f;

    [Header("Flocking Properties")] public Transform leaderPos;
    public Lider leader;

    [Range(0, 10)] public float cohesionWeight = 1.0f;
    [Range(0, 10)] public float alignmentWeight = 1.0f;
    [Range(0, 10)] public float separationWeight = 1.5f;
    [Range(0, 10)] public float separationDistance = 2.0f;
    [Range(0, 10)] public float leaderFollowWeight = 1.5f;
    [Range(0, 10)] public float avoidWeight = 1.5f;

    private List<IMovementBehaviour> _behaviors;

    [SerializeField] private LayerMask _obstacleMask;

    [SerializeField] private List<Node> _path = new(); // Camino a seguir

    public StateMachine stateMachine;

    private void Start()
    {
        leaderPos = leader.transform;

        InitializeStateMachine();
        InitializeBehaviors();
    }

    private void Update() //TODO: COSAS QUE SE CHEQUEAN SIEMPRE [NEIGHBORS, DEATH, ESCAPE]
    {
        DetectNeighbors();
        stateMachine.Update();
    }

    #region Initialize

    private void InitializeStateMachine()
    {
        stateMachine = gameObject.AddComponent<StateMachine>();

        stateMachine.AddState(NPCState.Await, new Await_NPC(this));
        stateMachine.AddState(NPCState.Walk, new Walk_NPC(this));
        stateMachine.AddState(NPCState.Attack, new Attack_NPC(this));
        stateMachine.AddState(NPCState.Escape, new Escape_NPC(this));
        stateMachine.AddState(NPCState.Recovery, new Recovery_NPC(this));
        stateMachine.AddState(NPCState.Death, new Death_NPC(this));

        stateMachine.ChangeState(NPCState.Walk);
    }

    private void InitializeBehaviors()
    {
        _behaviors = new List<IMovementBehaviour>
        {
            new CohesionBehaviour(),
            new AlignmentBehavior(),
            new SeparationBehavior(),
            new LeaderFollowingBehavior(),
            new ObstacleAvoidanceBehavior()
        };
    } //TODO: CLASE ENTITY

    #endregion

    #region MovementBehavior

    public string ObstacleAvoidance()
    {
        Vector3 leftRayOrigin = transform.position + transform.right * -0.5f;
        Vector3 rightRayOrigin = transform.position + transform.right * 0.5f;

        bool leftHit = Physics.Raycast(leftRayOrigin, transform.forward, 1f, _obstacleMask);
        bool rightHit = Physics.Raycast(rightRayOrigin, transform.forward, 1f, _obstacleMask);

        return leftHit ? "Left" : rightHit ? "Right" : "None";
    } //TODO: CLASE ENTITY

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
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    } //TODO: CLASE ENTITY

    #endregion

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

    #region LoS - FoV

    public bool HasLineOfSight()
    {
        // Lanza un raycast desde el personaje hacia el objetivo con una distancia máxima
        RaycastHit hit;
        Vector3 directionToTarget = (leaderPos.position - transform.position).normalized;
        float maxDistance = Vector3.Distance(transform.position, leaderPos.position); // Distancia máxima al objetivo

        // Si el raycast choca con algo, revisamos si es el objetivo
        if (Physics.Raycast(transform.position, directionToTarget, out hit, maxDistance))
        {
            // Verifica si el objeto golpeado es el líder
            if (hit.transform == leaderPos)
            {
                return true; // Línea de visión clara hacia el objetivo
            }
        }

        return false; // Algo bloquea la línea de visión
    }


    //bool HasFieldOfView()
    //{
    //    float angle = Vector3.Angle(transform.forward, directionToTarget);
    //
    //    return angle < ViewAngle / 2f;
    //}

    #endregion

    public void TakeDamage(int dmg) => Health -= dmg;

    #region Gizmos

    [SerializeField] private bool _onDrawGizmos;

    private void OnDrawGizmos()
    {
        if (!_onDrawGizmos) return;

        Vector3 forward = transform.forward;
        Vector3 leftRayOrigin = transform.position + transform.right * -0.5f;
        Vector3 rightRayOrigin = transform.position + transform.right * 0.5f;

        // Raycasts
        bool leftHit = Physics.Raycast(leftRayOrigin, forward, 1f, _obstacleMask);
        bool rightHit = Physics.Raycast(rightRayOrigin, forward, 1f, _obstacleMask);

        Gizmos.color = leftHit ? Color.red : Color.yellow;
        Gizmos.DrawLine(leftRayOrigin, leftRayOrigin + forward * 1.5f);

        Gizmos.color = rightHit ? Color.red : Color.yellow;
        Gizmos.DrawLine(rightRayOrigin, rightRayOrigin + forward * 1.5f);

        // Separation Distance
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, separationDistance);

        // Dirección del NPC
        if (Velocity.magnitude > 0.1f)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + Velocity);
        }

        // Dibujo del camino (_path_)
        if (_path != null && _path.Count > 0)
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < _path.Count - 1; i++)
            {
                Gizmos.DrawLine(_path[i].transform.position, _path[i + 1].transform.position);
            }
        }
    }

    #endregion
}

public enum NPCState
{
    Await,
    Walk,
    Attack,
    Escape,
    Recovery,
    Death
}