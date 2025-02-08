using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Node cameFrom;
    public List<Node> connections;

    public float gScore;
    public float hScore;

    private float rayLength = 2.5f; // Radio para buscar vecinos (ligeramente mayor que 1 unidad)
    [SerializeField] private LayerMask nodeLayer; // Capa específica para los nodos

    public float FScore()
    {
        return gScore + hScore;
    }

    private void Start() => FindConnections();

    public void FindConnections()
    {
        connections.Clear(); // Asegurarse de que no haya conexiones previas

        Vector3[] directions =
        {
            Vector3.right, // Derecha
            Vector3.left, // Izquierda
            Vector3.forward, // Arriba
            Vector3.back, // Abajo
            Vector3.right + Vector3.forward, // Arriba derecha
            Vector3.left + Vector3.forward, // Arriba izquierda
            Vector3.right + Vector3.back, // Abajo derecha
            Vector3.left + Vector3.back // Abajo izquierda
        };

        foreach (Vector3 direction in directions)
        {
            RaycastHit hit;
            // Raycast que solo detecta objetos en la capa "Node"
            if (Physics.Raycast(transform.position, direction, out hit, rayLength, nodeLayer))
            {
                Node neighbor = hit.collider.GetComponent<Node>();

                // Verificar que el nodo vecino no sea nulo, no sea este mismo nodo y no esté ocupado
                if (neighbor != null && neighbor != this)
                {
                    connections.Add(neighbor);
                    Debug.Log($"Nodo conectado: {neighbor.name}");
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (connections.Count > 0)
        {
            foreach (Node connectedNode in connections)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, connectedNode.transform.position);
            }
        }
    }
}