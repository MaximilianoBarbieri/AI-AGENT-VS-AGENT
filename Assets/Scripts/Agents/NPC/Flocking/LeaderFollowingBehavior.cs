using UnityEngine;
using static Utils;

public class LeaderFollowingBehavior : IFlockingBehaviour
{
    public Vector3 CalculateSteeringVelocity(NPC npc)
    {
        Vector3 toLeader = npc.leaderPos.position - npc.transform.position;
        float distance = toLeader.magnitude;

        if (distance < npc.minDistanceLeader)
            return Vector3.zero;

        float slowingRadius = 3f; 
        float speedFactor = Mathf.Clamp01(distance / slowingRadius);

        Vector3 desiredVelocity = toLeader.normalized * npc.MoveSpeed * speedFactor;
        return desiredVelocity * npc.leaderFollowWeight;
    }
}