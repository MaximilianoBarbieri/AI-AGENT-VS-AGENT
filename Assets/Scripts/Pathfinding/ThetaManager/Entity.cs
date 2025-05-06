using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Entity : MonoBehaviour, IThetaMovement
{
    [SerializeField] protected LayerMask nodeLayer;

    [HideInInspector] public List<Node> path = new();
    [HideInInspector] public Node startNode;
    [HideInInspector] public Node targetNode;

    [SerializeField] protected LayerMask _obstacleMask;
    
    public static Action<Node, Leader> OnSetTargetNode;

    public Node GetCurrentNode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 2f, nodeLayer);
        foreach (var col in colliders)
        {
            Node node = col.GetComponent<Node>();
            if (node != null) return node;
        }

        return null;
    }

    public void SetTargetNode(Node tNode, Leader clickedLeader)
    {
        if (!ShouldReactToLeader(clickedLeader))
            return;

        startNode = GetCurrentNode();
        targetNode = tNode;
        path = ThetaManager.FindPath(startNode, targetNode);
    }

    public virtual void MoveAlongPath(float moveSpeed)
    {
        if (path.Count == 0) return;

        transform.position =
            Vector3.MoveTowards(transform.position, path[0].transform.position, moveSpeed * Time.deltaTime);
        transform.LookAt(path[0].transform.position);

        if (Vector3.Distance(transform.position, path[0].transform.position) < 0.1f)
        {
            path.RemoveAt(0);
        }
    }

    public abstract bool HasLineOfSight();
    public abstract string ObstacleAvoidance();
    public abstract void TakeDamage(int dmg);
    
    public abstract bool ShouldReactToLeader(Leader clickedLeader);
}