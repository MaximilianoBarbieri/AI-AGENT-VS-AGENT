using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignmentBehavior : IFlockingBehaviour
{
    public Vector3 CalculateSteeringVelocity(NPC npc)
    {
        Vector3 avgDirection = Vector3.zero;
        int count = 0;

        foreach (var neighbor in npc.neighbors)
        {
            if (neighbor == npc) continue;
            avgDirection += neighbor.Velocity;
            count++;
        }

        if (count == 0) return Vector3.zero;

        avgDirection /= count;
        return avgDirection.normalized * npc.alignmentWeight;
    }
}