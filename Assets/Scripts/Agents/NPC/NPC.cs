using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public float Health { get; set; }

    public const float Damage = 5;
    public const float RegenerationLife = 0.5f;
    private const float MoveSpeed = 5f;

    [Header("Flocking properties")] public Transform leader;

    [Range(0, 10)] public float cohesionWeight = 1.0f;

    [Range(0, 10)] public float alignmentWeight = 1.0f;

    [Range(0, 10)] public float separationWeight = 1.5f;
    [Range(0, 10)] public float separationDistance = 2.0f;

    [Range(0, 10)] public float leaderFollowWeight = 1.5f;
    [Range(0, 100)] public float avoidWeight = 1.5f;

    [SerializeField] private bool _onDrawGizmos;

    public Vector3 Velocity { get; private set; }
    private List<IMovementBehaviour> _behaviors;

    [HideInInspector] public LayerMask obstacleMask;
    [HideInInspector] public List<NPC> neighbors = new();
    [HideInInspector] public StateMachine stateMachine;

    private void Start()
    {
        stateMachine = gameObject.AddComponent<StateMachine>();

        stateMachine.AddState(NPCState.Walk, new Walk_NPC(this));
        stateMachine.AddState(NPCState.Attack, new Attack_NPC(this));
        stateMachine.AddState(NPCState.Escape, new Escape_NPC(this));
        stateMachine.AddState(NPCState.Recovery, new Recovery_NPC(this));
        stateMachine.AddState(NPCState.Death, new Death_NPC(this));

        stateMachine.ChangeState(NPCState.Recovery);

        _behaviors = new List<IMovementBehaviour>
        {
            new CohesionBehaviour(), // COHESION
            new AlignmentBehavior(), // ALINEACION
            new SeparationBehavior(), // SEPARACION
            new LeaderFollowingBehavior(),
            new ObstacleAvoidanceBehavior(1.5f, avoidWeight, obstacleMask)
        };
    }

    private void Update() => DetectNeighbors();

    public void Flocking()
    {
        Vector3 movement;

        Vector3 obstacleAvoidance = _behaviors
            .Find(behavior => behavior is ObstacleAvoidanceBehavior)
            ?.CalculateSteeringVelocity(this) ?? Vector3.zero;

        if (obstacleAvoidance.magnitude > 0.1f) // Si detecta un obstáculo, priorizar la evasión
        {
            movement = obstacleAvoidance;
        }
        else
        {
            movement = new SeparationBehavior().CalculateSteeringVelocity(this);

            foreach (var behavior in _behaviors)
            {
                if (!(behavior is SeparationBehavior) && !(behavior is ObstacleAvoidanceBehavior))
                    movement += behavior.CalculateSteeringVelocity(this);
            }

            Quaternion targetRotation = Quaternion.LookRotation(Velocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        Velocity = movement.normalized * MoveSpeed;
        transform.position += Velocity * Time.deltaTime;
    }

    //TODO: Desde el estado Walk, debera seguir al Leader con Flocking.
    //TODO; Si no puede seguirlo por Flocking, debera obtener su currentNode e ir hacia ahi por medio de Theta.
    //TODO: Tambien, para poder escapar, debera buscar un Nodo Libre e ir a refugiarse a esa posicion.

    public void FollowThetaAtLeader()
    {
    }

    private void DetectNeighbors()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, separationDistance);
        HashSet<NPC> newNeighbors = new HashSet<NPC>();

        foreach (Collider col in colliders)
        {
            NPC neighbor = col.GetComponent<NPC>();
            if (neighbor != null && neighbor != this)
            {
                newNeighbors.Add(neighbor);
            }
        }

        if (newNeighbors.Count != neighbors.Count || !newNeighbors.SetEquals(neighbors))
            neighbors = new List<NPC>(newNeighbors);
    }

    public void TakeDamage(int dmg) => Health -= dmg;

    #region Gizmos

    private void OnDrawGizmos()
    {
        if (!_onDrawGizmos) return;

        Vector3 forward = transform.forward;
        Vector3 leftRayOrigin = transform.position + transform.right * -0.5f;
        Vector3 rightRayOrigin = transform.position + transform.right * 0.5f;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(leftRayOrigin, leftRayOrigin + forward * 1.5f);
        Gizmos.DrawLine(rightRayOrigin, rightRayOrigin + forward * 1.5f);
    }

    #endregion
}

public enum NPCState
{
    Walk,
    Attack,
    Escape,
    Recovery,
    Death
}