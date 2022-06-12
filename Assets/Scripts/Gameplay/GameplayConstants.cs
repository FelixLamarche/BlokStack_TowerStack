using UnityEngine;

public static class GameplayConstants
{
    public const float playerObstacleDepth = -2;
    public const float blockDepth = 0;

    public static readonly Vector3 platformSpawnPoint;

    static GameplayConstants()
    {
        platformSpawnPoint = new Vector3(0, 1, blockDepth);
    }
}