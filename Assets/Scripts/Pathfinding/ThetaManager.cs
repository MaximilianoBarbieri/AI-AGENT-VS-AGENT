using System.Collections.Generic;
using UnityEngine;

public class ThetaManager : MonoBehaviour
{
    public static List<Node> FindPath(Node startNode, Node targetNode)
    {
        if (startNode == null || targetNode == null)
        {
            Debug.LogError("Start or target node is null.");
            return new List<Node>();
        }

        PriorityQueue<Node> openSet = new PriorityQueue<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        Dictionary<Node, float> gScore = new Dictionary<Node, float>();
        Dictionary<Node, float> fScore = new Dictionary<Node, float>();

        gScore[startNode] = 0f;
        fScore[startNode] = Heuristic(startNode, targetNode);
        openSet.Enqueue(startNode, fScore[startNode]);

        while (openSet.Count > 0)
        {
            Node current = openSet.Dequeue();

            if (current == targetNode)
                return ReconstructPath(cameFrom, current);

            closedSet.Add(current);

            foreach (Node neighbor in current.connections)
            {
                if (closedSet.Contains(neighbor))
                    continue;

                Node parent = cameFrom.ContainsKey(current) ? cameFrom[current] : current;
                float tentativeG;
                Node tentativeParent;

                if (HasLineOfSight(parent, neighbor))
                {
                    tentativeParent = parent;
                    tentativeG = gScore[parent] + Heuristic(parent, neighbor);
                }
                else
                {
                    tentativeParent = current;
                    tentativeG = gScore[current] + Heuristic(current, neighbor);
                }

                if (!gScore.ContainsKey(neighbor) || tentativeG < gScore[neighbor])
                {
                    cameFrom[neighbor] = tentativeParent;
                    gScore[neighbor] = tentativeG;
                    fScore[neighbor] = tentativeG + Heuristic(neighbor, targetNode);

                    if (!openSet.Contains(neighbor))
                        openSet.Enqueue(neighbor, fScore[neighbor]);
                }
            }
        }

        return new List<Node>(); // No path found
    }

    private static float Heuristic(Node a, Node b)
    {
        return Vector3.Distance(a.transform.position, b.transform.position);
    }

    private static bool HasLineOfSight(Node from, Node to)
    {
        Vector3 dir = to.transform.position - from.transform.position;
        float dist = dir.magnitude;
        return !Physics.Raycast(from.transform.position, dir.normalized, dist);
    }

    private static List<Node> ReconstructPath(Dictionary<Node, Node> cameFrom, Node current)
    {
        List<Node> path = new List<Node> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, current);
        }
        return path;
    }
}
