using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Lider : MonoBehaviour
{
    [SerializeField] private LayerMask nodeLayer; // Capa donde están los nodos
    [SerializeField] private float moveSpeed = 5f; // Velocidad de movimiento del líder
    [SerializeField] private MouseButton _buttonDown;

    private Node targetNode; // Nodo al que se dirige
    private List<Node> path = new(); // Camino a seguir

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
            }
        }
    }

    private void MoveAlongPath()
    {
        if (path.Count > 0)
        {
            Node nextNode = path[0];
            transform.position = Vector3.MoveTowards(transform.position, nextNode.transform.position,
                moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, nextNode.transform.position) < 0.1f)
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
            if (node != null)
            {
                return node;
            }
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
}

#region Completed

//● Crear una escena con paredes de por medio, obstáculos pequeños y límite en sus bordes.
//● El Líder tendrá que dirigirse al lugar más cercano en donde se haga click con
//el mouse.
//

#endregion

#region TODO

//● Crear NPCs para ambos bandos y que cada uno tenga un líder que será
//controlado por el usuario (al menos 5 NPCs por bando).
//
//● Las entidades que no son controladas por el usuario deberán utilizar un
//algoritmo que incluya el sistema de Flocking para moverse cerca del líder
//(pueden utilizar solo Leader Following (Arrive + Separation) si se lo desea).
//
//● Las entidades utilizarán Line of Sight (no FoV) para chequear si pueden
//llegar a su próximo destino; En el caso de que no pueda llegar a su lugar de
//destino debido a que hay una pared entre medio se deberá utilizar el algoritmo
//Theta* para calcular un camino y poder seguir haciendo lo deseado (Por
//ejemplo: El líder en donde se hizo click, o los npcs hacia donde esta el lider).
//
//● Punto Extra: Si el camino que se está siguiendo es obstruido por una pared
//(no los obstáculos pequeños) se deberá recalcular un camino nuevo.
//
//● Desarrollar las FSM necesarias tanto para los líderes (al menos 3 estados)
//como las otras unidades para que puedan atacar a sus adversarios (al menos 3
//estados). A su vez, tener en cuenta los siguientes puntos:
//○ Los enemigos sólo podrán atacar a otros si los tienen en su Área de
//Visión (Field of View).
//○ Todos los NPCs valorarán su vida antes que la de su batallón. Es
//decir, huirán en caso de tener poca HP e intentarán buscar lugares a
//salvo.
//
//● Todos los agentes en la escena deberán esquivar los obstáculos pequeños*
//que se encuentren en su camino con algún algoritmo de Obstacle Avoidance.
//*doble o menor tamaño que los agentes. 

#endregion