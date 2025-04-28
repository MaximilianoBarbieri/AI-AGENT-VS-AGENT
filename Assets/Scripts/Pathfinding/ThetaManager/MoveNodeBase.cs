using System.Collections.Generic;
using UnityEngine;

public class MoveNodeBase : MonoBehaviour, IMoveNode
{
    [SerializeField] protected LayerMask nodeLayer;

    protected List<Node> Path = new();
    protected Node TargetNode;

    protected const int MoveSpeed = 5;

    public Node GetCurrentNode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 1f, nodeLayer);
        foreach (var col in colliders)
        {
            Node node = col.GetComponent<Node>();
            if (node != null) return node;
        }

        return null;
    }

    public Node FindNearestNode(Vector3 pos)
    {
        Collider[] colliders = Physics.OverlapSphere(pos, 3f, nodeLayer);
        Node nearestNode = null;
        float minDistance = Mathf.Infinity;

        foreach (var col in colliders)
        {
            Node node = col.GetComponent<Node>();
            if (node != null)
            {
                float distance = Vector3.Distance(pos, node.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestNode = node;
                }
            }
        }

        return nearestNode;
    }

    public virtual void SetTargetNode(Vector3 pos)
    {
        Node nearestNode = FindNearestNode(pos);
        if (nearestNode != null)
        {
            TargetNode = nearestNode;
            Path = ThetaManager.FindPath(GetCurrentNode(), TargetNode);
        }
    }

    public virtual void MoveAlongPath()
    {
        if (Path.Count == 0) return;

        transform.position =
            Vector3.MoveTowards(transform.position, Path[0].transform.position, MoveSpeed * Time.deltaTime);
        transform.LookAt(Path[0].transform.position);

        if (Vector3.Distance(transform.position, Path[0].transform.position) < 0.1f)
        {
            Path.RemoveAt(0);
        }
    }
}