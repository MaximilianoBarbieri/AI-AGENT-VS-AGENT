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

        if (npc.ObjectInSight == "Left")
        {
            lastAvoidanceDirection = npc.transform.right;
            avoidanceTimer = avoidanceDuration; // Resetea el temporizador
        }
        else if (npc.ObjectInSight == "Right")
        {
            lastAvoidanceDirection = -npc.transform.right;
            avoidanceTimer = avoidanceDuration;
        }

        // Mantiene la dirección de evasión durante un tiempo después de detectar el obstáculo
        if (avoidanceTimer > 0)
        {
            avoidanceForce = lastAvoidanceDirection * npc.avoidWeight;
            avoidanceTimer -= Time.deltaTime;
        }

        return avoidanceForce;
    }
}