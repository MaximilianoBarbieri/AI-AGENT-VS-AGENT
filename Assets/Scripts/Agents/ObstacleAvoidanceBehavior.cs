using static Utils;
using UnityEngine;

public class ObstacleAvoidanceBehavior
{
    private Vector3 lastAvoidanceDirection = Vector3.zero;
    private float avoidanceTimer = 0f;
    private const float avoidanceDuration = 0.2f;

    public Vector3 CalculateSteeringVelocity(Entity entity)
    {
        Vector3 avoidanceForce = Vector3.zero;
        var dir = entity.ObstacleAvoidance();

        if (dir == LEFT_DIR)
        {
            lastAvoidanceDirection = entity.transform.right;
            avoidanceTimer = avoidanceDuration;
        }
        else if (dir == RIGHT_DIR)
        {
            lastAvoidanceDirection = -entity.transform.right;
            avoidanceTimer = avoidanceDuration;
        }

        if (avoidanceTimer > 0)
        {
            avoidanceForce = lastAvoidanceDirection * entity.avoidWeight;
            avoidanceTimer -= Time.deltaTime;
        }

        return avoidanceForce;
    }
}