using UnityEngine;

public interface IMoveNode
{
    Node GetCurrentNode();
    Node FindNearestNode(Vector3 pos);
    void SetTargetNode(Vector3 pos);
    void MoveAlongPath();
}