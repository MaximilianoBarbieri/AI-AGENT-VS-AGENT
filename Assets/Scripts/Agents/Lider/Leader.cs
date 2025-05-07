using UnityEngine;
using static Utils;

public class Leader : Entity
{
    [SerializeField] public Team myTeam;
    public Vector3 DirectTargetPos { get; private set; }

    public bool useMove;
    public bool useTheta;

    public Node safeZone;

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

    private void ChoosingNewGoal()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            DirectTargetPos = hit.point;

            bool hasSight = HasLineOfSight();

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


    public override bool HasLineOfSight()
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 direction = DirectTargetPos - origin;
        float distance = direction.magnitude;

        return !Physics.Raycast(origin, direction.normalized, distance, _obstacleMask);
    }

    public void Move()
    {
        Vector3 movement = Vector3.zero;

        Vector3 obstacleAvoidance = _obstacleAvoidance.CalculateSteeringVelocity(this);

        if (obstacleAvoidance.magnitude > 0.1f)
            movement = obstacleAvoidance;
        else
            movement = DirectTargetPos - transform.position;

        Velocity = movement.normalized * LEADER_MOVE_SPEED;
        transform.position += Velocity * Time.deltaTime;

        transform.LookAt(DirectTargetPos);
    }
}