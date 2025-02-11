using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleAvoidanceBehavior : IMovementBehaviour
{
    private float _avoidStrength;

    public ObstacleAvoidanceBehavior(float avoidStrength)
    {
        _avoidStrength = avoidStrength;
    }

    public Vector3 CalculateSteeringVelocity(NPC npc)
    {
        Vector3 avoidanceForce = Vector3.zero;

        if (npc.ObjectInSight == "Both")
        {
            npc.isFlocking = false; //Desactivo el Flocking
        }
        else if (npc.ObjectInSight == "Left")
        {
            npc.isFlocking = true;
            avoidanceForce += npc.transform.right * _avoidStrength; // Evita a la derecha
        }
        else if (npc.ObjectInSight == "Right")
        {
            npc.isFlocking = true;
            avoidanceForce -= npc.transform.right * _avoidStrength; // Evita a la izquierda
        }

        return avoidanceForce;
    }
}