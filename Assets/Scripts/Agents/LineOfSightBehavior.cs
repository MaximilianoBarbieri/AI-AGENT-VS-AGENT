using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOfSightBehavior : IFlockingBehaviour
{
    public int numRays = 5; // Cantidad de rayos a disparar
    public float fieldOfViewAngle = 45f; // Ángulo del campo de visión (en grados)

    public Vector3 CalculateSteeringVelocity(NPC npc)
    {
       // RaycastHit hit;
       //
       // // Direcciones de rayos dentro del campo de visión
       // for (int i = 0; i < numRays; i++)
       // {
       //     float angle = -fieldOfViewAngle / 2 + (i * fieldOfViewAngle / (numRays - 1)); // Calcula el ángulo de cada rayo
       //     Vector3 direction = Quaternion.Euler(0, angle, 0) * npc.Velocity.normalized; // Rotación para cada rayo
       //
       //     if (Physics.Raycast(npc.transform.position, direction, out hit, npc.SeparationDistance))
       //     {
       //         if (hit.collider.CompareTag("Obstacle"))
       //         {
       //             return Vector3.Reflect(npc.Velocity, hit.normal) * npc.LineOfSightWeight;
       //         }
       //     }
       // }

        return Vector3.zero;
    }

}