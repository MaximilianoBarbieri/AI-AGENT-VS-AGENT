public static class Utils
{
    //---------GENERAL---------
    public enum Team
    {
        Red,
        Blue
    }

    public const string LEFT_DIR = "Left";
    public const string RIGHT_DIR = "Right";
    public const string NONE_OBSTACLE = "None";

    public const float DISTANCE_OBSTACLE_AVOIDANCE = 0.25f;

    //---------NPC---------
    public enum NPCState
    {
        Await,
        Walk,
        Attack,
        Escape,
        Recovery,
        Death
    }

    public const int NPC_DAMAGE = 25;

    public const int NPC_MAX_HEALTH = 100;
    public const int NPC_MIN_HEALTH_TO_RECOVERY = 25;

    public const float NPC_CD_ATTACK_ORIG = 2f;
    public const float NPC_VIEWANGLE = 60;
    public const float NPC_VIEWRADIUS = 1.5f;

    public const float NPC_ORIGINAL_MOVE_SPEED = 2f;
    public const float NPC_RECOVERY_MOVE_SPEED = 4f;
    public const float NPC_REGENERATION_LIFE = 0.25f;

    //---------LEADER---------
    public enum LeaderState
    {
        Await,
        Attack,
        Walk
    }

    public const int LEADER_DAMAGE = 10;

    public const float LEADER_MOVE_SPEED = 4f;
    public const float LEADER_VIEW_RADIUS = 2f;
    public const float LEADER_CD_ATTACK_ORIG = 3f;
}