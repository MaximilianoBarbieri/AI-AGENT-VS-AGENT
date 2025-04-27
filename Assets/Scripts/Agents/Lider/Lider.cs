using Unity.VisualScripting;
using UnityEngine;

public class Lider : MoveNodeBase
{
    [SerializeField] private MouseButton _buttonDown;
    [SerializeField] public Team myTeam;
    [SerializeField] private bool _onDrawGizmos;

    public Vector3 safeZone;

    private void Update()
    {
        if (Input.GetMouseButtonDown((int)_buttonDown))
            SetTargetNodeFromMouse();

        MoveAlongPath();
    }

    private void SetTargetNodeFromMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            SetTargetNode(hit.point);
            Debug.Log("Líder se mueve hacia: " + TargetNode?.name);
        }
    }

    [SerializeField] private LayerMask obstacleLayer;

    private void OnDrawGizmos()
    {
        if (!_onDrawGizmos) return;

        // Dibujar el líder
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.5f); // Representar al líder con una esfera roja

        // Gizmo de los nodos en el camino
        if (Path.Count > 0)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < Path.Count - 1; i++)
            {
                Gizmos.DrawLine(Path[i].transform.position,
                    Path[i + 1].transform.position); // Línea que conecta los nodos en el camino
                Gizmos.DrawSphere(Path[i].transform.position, 0.2f); // Representa cada nodo del camino con una esfera
            }

            // Representar el último nodo del camino
            Gizmos.DrawSphere(Path[Path.Count - 1].transform.position, 0.2f);
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

    public enum Team
    {
        Red,
        Blue
    }
}