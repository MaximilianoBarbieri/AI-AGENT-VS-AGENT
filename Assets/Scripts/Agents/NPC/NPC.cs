using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [Header("Flocking properties")] public Transform leader;

    [Range(0, 10)] public float cohesionWeight = 1.0f;

    [Range(0, 10)] public float alignmentWeight = 1.0f;

    [Range(0, 10)] public float separationWeight = 1.5f;
    [Range(0, 10)] public float separationDistance = 2.0f;

    [Range(0, 10)] public float leaderFollowWeight = 1.5f;
    [Range(0, 100)] public float avoidWeight = 1.5f;

    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private bool _onDrawGizmos;

    public LayerMask obstacleMask;

    public Vector3 Velocity { get; set; }

    public List<NPC> neighbors = new();
    private List<IMovementBehaviour> _behaviors;
    public StateMachine stateMachine;

    private void Start()
    {
        stateMachine = gameObject.AddComponent<StateMachine>();

        _behaviors = new List<IMovementBehaviour>
        {
            new CohesionBehaviour(), // COHESION
            new AlignmentBehavior(), // ALINEACION
            new SeparationBehavior(), // SEPARACION
            new LeaderFollowingBehavior(),
        };

        _behaviors.Add(new ObstacleAvoidanceBehavior(1.5f, avoidWeight, obstacleMask));
    }

    private void Update()
    {
        DetectNeighbors();

        Vector3 movement = Vector3.zero;

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

        Velocity = movement.normalized * _moveSpeed;
        transform.position += Velocity * Time.deltaTime;
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

    public enum NPCState
    {
        Idle,
        Walk,
        Attack,
        Escape,
        Death
    }
}