using static Utils;
using UnityEngine;

public class ObstacleAvoidanceBehavior
{
    private Vector3 _lastAvoidanceDirection = Vector3.zero;
    private float _avoidanceTimer = 0f;
    private const float AvoidanceDuration = 0.2f;

    public Vector3 CalculateSteeringVelocity(Entity entity)
    {
        Vector3 avoidanceForce = Vector3.zero;
        var dir = entity.ObstacleAvoidance();

        if (dir == LEFT_DIR)
        {
            _lastAvoidanceDirection = entity.transform.right;
            _avoidanceTimer = AvoidanceDuration;
        }
        else if (dir == RIGHT_DIR)
        {
            _lastAvoidanceDirection = -entity.transform.right;
            _avoidanceTimer = AvoidanceDuration;
        }

        if (_avoidanceTimer > 0)
        {
            avoidanceForce = _lastAvoidanceDirection * entity.avoidWeight;
            _avoidanceTimer -= Time.deltaTime;
        }

        return avoidanceForce;
    }
}