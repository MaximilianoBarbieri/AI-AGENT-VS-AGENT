using UnityEngine;

public interface IThetaMovement
{
    Node GetCurrentNode();
    void SetTargetNode(Node tNode, Leader clickedLeader);
    void MoveAlongPath(float speed);
    public abstract bool ShouldReactToLeader(Leader clickedLeader);
}