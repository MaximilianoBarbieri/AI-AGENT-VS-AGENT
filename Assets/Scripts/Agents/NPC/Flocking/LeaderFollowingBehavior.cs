using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderFollowingBehavior : IFlockingBehaviour
{
    public Vector3 CalculateSteeringVelocity(NPC npc)
    {
        return (npc.leaderPos.position - npc.transform.position).normalized * npc.leaderFollowWeight;
    }
}