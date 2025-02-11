using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public float Health { get; set; } = 100f;
    public List<NPC> Neighbors { get; private set; } = new();
    public Vector3 Velocity { get; private set; }
    public string ObjectInSight { get; private set; }

    private const float Damage = 5;
    private const float MoveSpeed = 2f;
    public const float RegenerationLife = 0.5f;

    [Header("Flocking Properties")] public Transform leader;

    [Range(0, 10)] public float cohesionWeight = 1.0f;
    [Range(0, 10)] public float alignmentWeight = 1.0f;
    [Range(0, 10)] public float separationWeight = 1.5f;
    [Range(0, 10)] public float separationDistance = 2.0f;
    [Range(0, 10)] public float leaderFollowWeight = 1.5f;
    [Range(0, 10)] public float avoidWeight = 1.5f;

    private List<IMovementBehaviour> _behaviors;
    [SerializeField] private LayerMask _obstacleMask;
    [SerializeField] private LayerMask _nodeLayer;

    [SerializeField] private List<Node> _path = new(); // Camino a seguir

    public bool isFlocking;

    [HideInInspector] public StateMachine stateMachine;

    private void Start()
    {
        InitializeStateMachine();
        InitializeBehaviors();
    }

    private void Update()
    {
        ObjectInSight = LineOfSight();

        DetectNeighbors();
    }

    private void InitializeStateMachine()
    {
        stateMachine = gameObject.AddComponent<StateMachine>();

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
            new ObstacleAvoidanceBehavior(avoidWeight)
        };
    }

    public void TakeDamage(int dmg) => Health -= dmg;

    private string LineOfSight()
    {
        Vector3 leftRayOrigin = transform.position + transform.right * -0.5f;
        Vector3 rightRayOrigin = transform.position + transform.right * 0.5f;

        bool leftHit = Physics.Raycast(leftRayOrigin, transform.forward, out RaycastHit leftInfo, 1f, _obstacleMask);
        bool rightHit = Physics.Raycast(rightRayOrigin, transform.forward, out RaycastHit rightInfo, 1f,
            _obstacleMask);

        // Verifica si ambos, solo uno o ninguno de los raycasts colisionaron
        if (leftHit && rightHit)
        {
            return "Both"; // Ambos raycasts colisionaron
        }
        else if (leftHit)
        {
            return "Left"; // Solo el raycast izquierdo colisionó
        }
        else if (rightHit)
        {
            return "Right"; // Solo el raycast derecho colisionó
        }

        return "None"; // Ningún raycast colisionó
    }

    #region Theta*

    private void FindPath(Node targetNode)
    {
        _path = ThetaManager.FindPath(GetCurrentNode(), targetNode);
        Debug.Log("FIND PATH    ");
    }

    private bool isPathRecalculated = false;

    public void MoveAlongPath()
    {
        if (_path.Count > 0)
        {
            // Solo recalcular si hay un obstáculo y no lo hemos hecho ya
            if (LineOfSight() == "Both" && !isPathRecalculated)
            {
                FindPath(_path[_path.Count - 1]);

            Debug.Log("Moviéndose hacia el nodo: " + _path[0].name); // Debug
            transform.position = Vector3.MoveTowards(transform.position, _path[0].transform.position,
                MoveSpeed * Time.deltaTime);
            transform.LookAt(_path[0].transform.position);

            if (Vector3.Distance(transform.position, _path[0].transform.position) < 0.1f)
            {
                Debug.Log("Nodo alcanzado: " + _path[0].name); // Debug
                _path.RemoveAt(0);
            }
        }
        else
        {
            Debug.LogWarning("El camino está vacío."); // Debug
        }
    }


    private Node GetCurrentNode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 2f, _nodeLayer);

        foreach (Collider col in colliders)
        {
            Node node = col.GetComponent<Node>();

            if (node != null) return node;
        }

        return null;
    }

    #endregion

    #region Flocking

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
        RotateTowardsMovement();
    }

    private void RotateTowardsMovement()
    {
        if (Velocity.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(Velocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
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

        if (newNeighbors.Count != Neighbors.Count || !newNeighbors.SetEquals(Neighbors))
            Neighbors = new List<NPC>(newNeighbors);
    }

    #endregion

    #region Gizmos

    [SerializeField] private bool _onDrawGizmos;

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

    private void OnEnable()
    {
        Lider.OnLeaderMove += FindPath;
    }

    private void OnDisable()
    {
        Lider.OnLeaderMove -= FindPath;
    }
}

public enum NPCState
{
    Walk,
    Attack,
    Escape,
    Recovery,
    Death
}