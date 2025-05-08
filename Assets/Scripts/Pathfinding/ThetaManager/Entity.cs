using System.Collections.Generic;
using UnityEngine;
using static Utils;

public abstract class Entity : MonoBehaviour, IThetaMovement
{
    public float Health { get; set; } = 100f;
    public Vector3 Velocity { get; set; }
    protected float MaxSpeed { get; set; }
    protected float ViewRadius { get; set; }

    public List<Node> path = new();
    public StateMachine stateMachine;

    [Header("LAYERS")] 
    
    [SerializeField] protected LayerMask nodeLayer;
    [SerializeField] protected LayerMask _obstacleMask;

    [Header("OBSTACLE AVOIDANCE")] 
    
    [Range(0, 5)]public float avoidWeight;
    protected ObstacleAvoidanceBehavior _obstacleAvoidance;

    public Node GetCurrentNode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 2f, nodeLayer);

        Node closestNode = null;
        float shortestDistance = Mathf.Infinity;

        foreach (var col in colliders)
        {
            Node node = col.GetComponent<Node>();

            if (node != null)
            {
                float distance = Vector3.Distance(transform.position, node.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    closestNode = node;
                }
            }
        }

        return closestNode;
    }

    public void MoveAlongPath(float moveSpeed)
    {
        if (path.Count == 0) return;

        transform.position =
            Vector3.MoveTowards(transform.position, path[0].transform.position, moveSpeed * Time.deltaTime);
        transform.LookAt(path[0].transform.position);

        if (Vector3.Distance(transform.position, path[0].transform.position) < 0.1f)
            path.RemoveAt(0);
    }

    public string ObstacleAvoidance()
    {
        Vector3 leftRayOrigin = transform.position + transform.right * -0.5f;
        Vector3 rightRayOrigin = transform.position + transform.right * 0.5f;

        bool leftHit = Physics.Raycast(leftRayOrigin, transform.forward, DISTANCE_OBSTACLE_AVOIDANCE,
            _obstacleMask);
        bool rightHit = Physics.Raycast(rightRayOrigin, transform.forward, DISTANCE_OBSTACLE_AVOIDANCE,
            _obstacleMask);

        return leftHit ? LEFT_DIR : rightHit ? RIGHT_DIR : NONE_OBSTACLE;
    }


    protected Vector3 Seek(Vector3 targetPos) //MOVIMIENTO PARA LIDER
    {
        return Seek(targetPos, MaxSpeed);
    }

    private Vector3 Seek(Vector3 targetPos, float speed)
    {
        Vector3 desired = (targetPos - transform.position).normalized * speed;

        Vector3 steering = desired - Velocity;

        steering = Vector3.ClampMagnitude(steering, 3 * Time.deltaTime);

        return steering;
    }

    protected Vector3 Arrive(Vector3 targetPos) //MOVIMIENTO DE NPC A LIDER
    {
        float dist = Vector3.Distance(transform.position, targetPos);
        if (dist > ViewRadius) return Seek(targetPos);

        return Seek(targetPos, MaxSpeed * (dist / ViewRadius));
    }

    public bool HasLineOfSight(Vector3 target)
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 direction = target - origin;
        float distance = direction.magnitude;

        return !Physics.Raycast(origin, direction.normalized, distance, _obstacleMask);
    }

    public abstract void Move();
}