using UnityEngine;

public class LeaderFollowingBehavior : IFlockingBehaviour
{
    //No es un movimiento del propio de Flocking, pero se utiliza para calcular hacia donde tiene que ir

    public Vector3 CalculateSteeringVelocity(NPC npc)
    {
        Vector3 toLeader = npc.LeaderPos.position - npc.transform.position;
        float distance = toLeader.magnitude;

        if (distance < npc.minDistanceLeader)
            return Vector3.zero;

        float slowingRadius = 3f;
        float speedFactor = Mathf.Clamp01(distance / slowingRadius);

        Vector3 desiredVelocity = toLeader.normalized * npc.MoveSpeed * speedFactor;
        return desiredVelocity * npc.leaderFollowWeight;
    }
}