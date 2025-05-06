public static class Utils
{
    public enum Team
    {
        Red,
        Blue
    }

    //NPC
    public enum NPCState
    {
        Await,
        Walk,
        Attack,
        Escape,
        Recovery,
        Death
    }

    public const float NPC_CD_ATTACK_ORIG = 2f;

    public const int NPC_DAMAGE = 25;

    public const float NPC_VIEWANGLE = 60;
    public const float NPC_VIEWRADIUS = 5;

    public const int NPC_DISTANCE_OBSTACLE_AVOIDANCE = 1;

    public const float NPC_REGENERATION_LIFE = 0.25f;

    public const float NPC_ORIGINAL_MOVE_SPEED = 2f;
    public const float NPC_AWAIT_MOVE_SPEED = 0.5f;
    public const float NPC_ROTATION_SPEED = 2f;

    //LEADER
    public enum LeaderState
    {
        Await,
        Attack,
        Walk
    }

    public const float LEADER_CD_ATTACK_ORIG = 3f;

    
    public const float LEADER_MOVE_SPEED = 4f;
    public const int LEADER_DAMAGE = 10;
    public const float LEADER_RADIUS_ATTACK = 3f;
}