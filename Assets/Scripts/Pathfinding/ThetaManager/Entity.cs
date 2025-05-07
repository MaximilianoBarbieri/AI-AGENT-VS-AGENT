using System.Collections.Generic;
using UnityEngine;
using static Utils;

public abstract class Entity : MonoBehaviour, IThetaMovement
{
    public float Health = 100f;
    public Vector3 Velocity { get; set; }
    public float MoveSpeed { get; set; }

    public StateMachine stateMachine;

    [SerializeField] protected LayerMask nodeLayer;

    public List<Node> path = new();

    [SerializeField] protected LayerMask _obstacleMask;

    protected ObstacleAvoidanceBehavior _obstacleAvoidance;

    [Range(0, 5)] public float avoidWeight;

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

    public abstract bool HasLineOfSight();
}