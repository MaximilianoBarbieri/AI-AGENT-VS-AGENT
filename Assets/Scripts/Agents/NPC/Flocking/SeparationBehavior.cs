using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeparationBehavior : IFlockingBehaviour
{
    public Vector3 CalculateSteeringVelocity(NPC npc)
    {
        Vector3 separationForce = Vector3.zero;

        foreach (NPC neighbor in npc.Neighbors)
        {
            Vector3 diff = npc.transform.position - neighbor.transform.position;
            float distance = diff.magnitude;

            if (distance < npc.separationDistance)
                separationForce += diff.normalized / distance;
        }

        return separationForce.normalized * npc.separationWeight;
    }
}