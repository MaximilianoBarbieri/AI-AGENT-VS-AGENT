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

        gScore[startNode] = 0;
        fScore[startNode] = Vector3.Distance(startNode.transform.position, targetNode.transform.position);
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

                // Nodo padre de referencia
                Node parent = cameFrom.ContainsKey(current) ? cameFrom[current] : current;

                // Línea de visión directa desde el padre al vecino
                if (HasLineOfSight(parent, neighbor))
                {
                    float newG = gScore[parent] + Vector3.Distance(parent.transform.position, neighbor.transform.position);

                    if (!gScore.ContainsKey(neighbor) || newG < gScore[neighbor])
                    {
                        cameFrom[neighbor] = parent;
                        gScore[neighbor] = newG;
                        fScore[neighbor] = newG + Vector3.Distance(neighbor.transform.position, targetNode.transform.position);

                        if (!openSet.Contains(neighbor))
                            openSet.Enqueue(neighbor, fScore[neighbor]);
                    }
                }
                else
                {
                    float newG = gScore[current] + Vector3.Distance(current.transform.position, neighbor.transform.position);

                    if (!gScore.ContainsKey(neighbor) || newG < gScore[neighbor])
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = newG;
                        fScore[neighbor] = newG + Vector3.Distance(neighbor.transform.position, targetNode.transform.position);

                        if (!openSet.Contains(neighbor))
                            openSet.Enqueue(neighbor, fScore[neighbor]);
                    }
                }
            }
        }

        return new List<Node>(); // Si no se encuentra camino
    }

    private static bool HasLineOfSight(Node from, Node to)
    {
        Vector3 direction = to.transform.position - from.transform.position;
        float distance = direction.magnitude;

        // Opcional: ajustá el layerMask si querés ignorar ciertas capas
        return !Physics.Raycast(from.transform.position, direction.normalized, distance);
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
