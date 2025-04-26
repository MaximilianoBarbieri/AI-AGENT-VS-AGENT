using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleAvoidanceBehavior : IMovementBehaviour
{
    private Vector3 lastAvoidanceDirection = Vector3.zero;
    private float avoidanceTimer = 0f;
    private const float avoidanceDuration = 0.2f; // Segundos que mantiene la evasión

    public Vector3 CalculateSteeringVelocity(NPC npc)
    {
        Vector3 avoidanceForce = Vector3.zero;
        var dir = npc.ObstacleAvoidance();

        if (dir == "Left")
        {
            lastAvoidanceDirection = npc.transform.right;
            avoidanceTimer = avoidanceDuration; // Resetea el temporizador
        }
        else if (dir == "Right")
        {
            lastAvoidanceDirection = -npc.transform.right;
            avoidanceTimer = avoidanceDuration;
        }

        if (avoidanceTimer > 0)
        {
            avoidanceForce = lastAvoidanceDirection * npc.avoidWeight;
            avoidanceTimer -= Time.deltaTime;
        }

        return avoidanceForce;
    }
}