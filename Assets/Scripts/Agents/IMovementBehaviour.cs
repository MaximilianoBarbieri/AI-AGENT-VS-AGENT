using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovementBehaviour
{
    Vector3 CalculateSteeringVelocity(NPC npc);
}