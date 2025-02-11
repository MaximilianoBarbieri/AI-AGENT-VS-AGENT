using System.Collections.Generic;
using UnityEngine;


public class ThetaManager : MonoBehaviour
{
    public static List<Node> FindPath(Node startNode, Node targetNode)
    {
        if (startNode == null || targetNode == null)
        {
            Debug.LogError("Start or target node is null.");
            return new List<Node>(); // Devuelve una lista vacía si los nodos son nulos
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

                float tentativeGScore = gScore[current] +
                                        Vector3.Distance(current.transform.position, neighbor.transform.position);

                if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] +
                                       Vector3.Distance(neighbor.transform.position, targetNode.transform.position);

                    if (!openSet.Contains(neighbor))
                        openSet.Enqueue(neighbor, fScore[neighbor]);
                }
            }
        }

        return new List<Node>(); // Retorna un camino vacío si no encuentra solución
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