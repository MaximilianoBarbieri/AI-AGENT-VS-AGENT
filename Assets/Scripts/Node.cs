using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public List<Node> connections;

    private float rayLength = 2.5f;
    [SerializeField] private LayerMask nodeLayer;

    [SerializeField] private bool _onDrawGizmos;

    private void Start() => FindConnections();

    private void FindConnections()
    {
        connections.Clear();

        Vector3[] directions =
        {
            Vector3.right,
            Vector3.left,
            Vector3.forward,
            Vector3.back,
            Vector3.right + Vector3.forward,
            Vector3.left + Vector3.forward,
            Vector3.right + Vector3.back,
            Vector3.left + Vector3.back
        };

        foreach (Vector3 direction in directions)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, direction, out hit, rayLength))
            {
                if ((nodeLayer.value & (1 << hit.collider.gameObject.layer)) == 0)
                {
//                    Debug.Log($"Objeto detectado fuera de NodeLayer: {hit.collider.name}");
                    continue;
                }

                Node neighbor = hit.collider.GetComponent<Node>();


                if (neighbor != null && neighbor != this)
                {
                    connections.Add(neighbor);
                    //   Debug.Log($"Nodo conectado: {neighbor.name}");
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!_onDrawGizmos) return;

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