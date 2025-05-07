using UnityEngine;

public interface IFlockingBehaviour
{
    Vector3 CalculateSteeringVelocity(NPC npc);
}