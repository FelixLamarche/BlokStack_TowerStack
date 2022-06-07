using UnityEngine;

public static class GameplayConstants
{
    public const float obstacleDepth = -2;
    public static readonly Vector3 platformSpawnPoint;

    static GameplayConstants()
    {
        platformSpawnPoint = new Vector3(0, 1, 0);
    }
}