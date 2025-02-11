using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Lider : MonoBehaviour
{
    [SerializeField] private LayerMask nodeLayer; // Capa donde están los nodos
    [SerializeField] private LayerMask obstacleLayer; // Capa donde están los obstáculos (paredes)
    [SerializeField] private float moveSpeed = 5f; // Velocidad de movimiento del líder
    [SerializeField] private MouseButton _buttonDown;

    [SerializeField] private bool _onDrawGizmos;

    public Node targetNode; // Nodo al que se dirige
    private List<Node> path = new(); // Camino a seguir

    public static event Action<Node> OnLeaderMove; // Evento para notificar a los NPCs

    private void Update()
    {
        if (Input.GetMouseButtonDown((int)_buttonDown))
            SetTargetNode();

        MoveAlongPath();
    }

    private void SetTargetNode()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Node nearestNode = FindNearestNode(hit.point);

            if (nearestNode != null)
            {
                targetNode = nearestNode;
                path = ThetaManager.FindPath(GetCurrentNode(), targetNode);
                Debug.Log("Líder se mueve hacia: " + targetNode.name); // Debug
                OnLeaderMove?.Invoke(targetNode);
            }
        }
    }

    private void MoveAlongPath()
    {
        if (path.Count > 0)
        {
            transform.position =
                Vector3.MoveTowards(transform.position, path[0].transform.position, moveSpeed * Time.deltaTime);
            transform.LookAt(path[0].transform.position);

            if (Vector3.Distance(transform.position, path[0].transform.position) < 0.1f)
            {
                path.RemoveAt(0);
            }
        }
    }


    private Node GetCurrentNode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 1f, nodeLayer);
        foreach (Collider col in colliders)
        {
            Node node = col.GetComponent<Node>();
            if (node != null) return node;
        }

        return null;
    }

    private Node FindNearestNode(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, 3f, nodeLayer);
        Node nearestNode = null;
        float minDistance = Mathf.Infinity;

        foreach (Collider col in colliders)
        {
            Node node = col.GetComponent<Node>();
            if (node != null)
            {
                float distance = Vector3.Distance(position, node.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestNode = node;
                }
            }
        }

        return nearestNode;
    }

    private void OnDrawGizmos()
    {
        if (!_onDrawGizmos) return;

        // Dibujar el líder
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.5f); // Representar al líder con una esfera roja

        // Gizmo de los nodos en el camino
        if (path.Count > 0)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < path.Count - 1; i++)
            {
                Gizmos.DrawLine(path[i].transform.position,
                    path[i + 1].transform.position); // Línea que conecta los nodos en el camino
                Gizmos.DrawSphere(path[i].transform.position, 0.2f); // Representa cada nodo del camino con una esfera
            }

            // Representar el último nodo del camino
            Gizmos.DrawSphere(path[path.Count - 1].transform.position, 0.2f);
        }

        // Gizmos para los raycasts (LoS)
        Vector3 forward = transform.forward;
        Vector3 raycast1End = transform.position + forward * 5f; // Raycast hacia adelante
        Vector3 raycast2End =
            transform.position + (forward + transform.right * 1.2f) * 5f; // Raycast desplazado hacia un lado

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, raycast1End); // Raycast 1
        Gizmos.DrawLine(transform.position, raycast2End); // Raycast 2

        // Opcional: Colorear raycasts según si hay obstáculo o no
        RaycastHit hit1, hit2;
        if (Physics.Raycast(transform.position, forward, out hit1, 5f, obstacleLayer) ||
            Physics.Raycast(transform.position, forward + transform.right * 1.2f, out hit2, 5f, obstacleLayer))
        {
            Gizmos.color = Color.red; // Obstáculo detectado
        }
        else
        {
            Gizmos.color = Color.green; // No hay obstáculo
        }

        // Raycasts en color rojo si se detecta un obstáculo
        Gizmos.DrawLine(transform.position, raycast1End);
        Gizmos.DrawLine(transform.position, raycast2End);
    }
}