using UnityEngine;
using static Utils;

public class Leader : Entity
{
    [SerializeField] public Team myTeam;

    public float Health { get; set; } = 100f;

    public bool useMove;
    public bool useTheta;

    public StateMachine stateMachine;

    public Node safeZone;
    public Vector3 directTargetPos;
    public ObstacleAvoidanceBehavior ObstacleAvoidanceBehavior;

    private void Start()
    {
        stateMachine = gameObject.AddComponent<StateMachine>();

        stateMachine.AddState(LeaderState.Await, new Await_Leader(this));
        stateMachine.AddState(LeaderState.Walk, new Walk_Leader(this));
        stateMachine.AddState(LeaderState.Attack, new Attack_Leader(this));

        stateMachine.ChangeState(LeaderState.Await);
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
            directTargetPos = hit.point;

            bool hasSight = HasLineOfSight();
            Debug.Log("¿Hay línea de visión directa? " + hasSight);

            Node node = hit.collider.GetComponent<Node>();

            if (node != null)
            {
                OnSetTargetNode?.Invoke(node, this);

                if (hasSight)
                    useMove = true;
                else if (targetNode != null)
                    useTheta = true;
            }
        }
    }


    public override bool HasLineOfSight()
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 direction = directTargetPos - origin;
        float distance = direction.magnitude;

        return !Physics.Raycast(origin, direction.normalized, distance, _obstacleMask);
    }

    public void Move()
    {
        string result = ObstacleAvoidance();

        if (result == "None")
        {
            Vector3 direction = (directTargetPos - transform.position).normalized;
            transform.position += direction * LEADER_MOVE_SPEED * Time.deltaTime;
            transform.LookAt(directTargetPos);
        }
    }

    public override string ObstacleAvoidance()
    {
        return "None";
    }

    public override void TakeDamage(int dmg)
    {
        Health -= dmg;

        if (Health <= 0)
            Destroy(gameObject);
    }

    public override bool ShouldReactToLeader(Leader clickedLeader)
    {
        return clickedLeader == this;
    }

    private void OnEnable()
    {
        OnSetTargetNode += SetTargetNode;
    }

    private void OnDisable()
    {
        OnSetTargetNode -= SetTargetNode;
    }
}