using UnityEngine;

public interface IMovementBehaviour
{
    Vector3 CalculateSteeringVelocity(NPC npc);
}