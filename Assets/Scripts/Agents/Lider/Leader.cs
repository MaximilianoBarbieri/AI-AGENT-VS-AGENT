using UnityEngine;
using static Utils;

public class Leader : Entity
{
    [SerializeField] public Team myTeam;
    public Vector3 DirectTargetPos { get; private set; }

    [Header("MOVE")] public bool useMove;
    public bool useTheta;

    public Node safeZone;

    public Leader()
    {
        MaxSpeed = LEADER_MOVE_SPEED;
        ViewRadius = LEADER_VIEW_RADIUS;
    }

    private void Start()
    {
        stateMachine = gameObject.AddComponent<StateMachine>();

        stateMachine.AddState(LeaderState.Await, new Await_Leader(this));
        stateMachine.AddState(LeaderState.Walk, new Walk_Leader(this));
        stateMachine.AddState(LeaderState.Attack, new Attack_Leader(this));

        stateMachine.ChangeState(LeaderState.Await);

        _obstacleAvoidance = new ObstacleAvoidanceBehavior();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown((int)myTeam))
            ChoosingNewGoal();
    }

    public override void Move()
    {
        Vector3 steering = Seek(DirectTargetPos);

        Vector3 obstacleAvoidance = _obstacleAvoidance.CalculateSteeringVelocity(this);

        if (obstacleAvoidance.magnitude > 0.1f)
            steering = obstacleAvoidance;

        Velocity += steering;
        Velocity = Vector3.ClampMagnitude(Velocity, MaxSpeed);

        transform.position += Velocity * Time.deltaTime;

        if (Velocity.sqrMagnitude > 0.01f)
            transform.forward = Velocity.normalized;
    }

    private void ChoosingNewGoal()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            DirectTargetPos = new Vector3(hit.point.x, 0, hit.point.z);

            bool hasSight = HasLineOfSight(DirectTargetPos);

            Node node = hit.collider.GetComponent<Node>();

            if (hasSight)
                useMove = true;
            else if (node != null)
            {
                path = ThetaManager.FindPath(GetCurrentNode(), node);
                useTheta = true;
            }
        }
    }
}