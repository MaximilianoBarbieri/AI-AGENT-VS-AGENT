using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CohesionBehaviour : IFlockingBehaviour
{
    public Vector3 CalculateSteeringVelocity(NPC npc)
    {
        Vector3 centerOfMass = Vector3.zero;
        int count = 0;

        foreach (var neighbor in npc.Neighbors)
        {
            if (neighbor == npc) continue;
            centerOfMass += neighbor.transform.position;
            count++;
        }

        if (count == 0) return Vector3.zero;

        centerOfMass /= count;
        return (centerOfMass - npc.transform.position).normalized * npc.cohesionWeight;
    }
}