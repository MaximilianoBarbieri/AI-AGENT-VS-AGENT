using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleAvoidanceBehavior : IMovementBehaviour
{
    private float _rayDistance;
    private float _avoidStrength;
    private LayerMask _mask;

    public ObstacleAvoidanceBehavior(float rayDistance, float avoidStrength, LayerMask mask)
    {
        _rayDistance = rayDistance;
        _avoidStrength = avoidStrength;
        _mask = mask;
    }

    public Vector3 CalculateSteeringVelocity(NPC npc)
    {
        Vector3 forward = npc.transform.forward; // Usamos forward en vez de Velocity.normalized
        Vector3 leftRayOrigin = npc.transform.position + npc.transform.right * -0.5f;
        Vector3 rightRayOrigin = npc.transform.position + npc.transform.right * 0.5f;

        bool leftHit = Physics.Raycast(leftRayOrigin, forward, out RaycastHit leftInfo, _rayDistance, _mask);
        bool rightHit = Physics.Raycast(rightRayOrigin, forward, out RaycastHit rightInfo, _rayDistance, _mask);

        Vector3 avoidanceForce = Vector3.zero;

        if (leftHit && rightHit)
        {
            // Si ambos rayos detectan obstáculos, moverse hacia la dirección opuesta al más cercano
            if (leftInfo.distance < rightInfo.distance)
                avoidanceForce += npc.transform.right * _avoidStrength;
            else
                avoidanceForce -= npc.transform.right * _avoidStrength;
        }
        else if (leftHit)
        {
            avoidanceForce += npc.transform.right * _avoidStrength; // Evita a la derecha
        }
        else if (rightHit)
        {
            avoidanceForce -= npc.transform.right * _avoidStrength; // Evita a la izquierda
        }

        return avoidanceForce;
    }
}