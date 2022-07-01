using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#pragma warning disable S1104 // Fields should not have public accessibility

[CreateAssetMenu(fileName = "gameParameters", menuName = "Game Parameters")]
public class GameParameters : ScriptableObject
{
    public bool spawnSpikes;
    public bool spawnPlayerObstacles;
}

#pragma warning restore S1104 // Fields should not have public accessibility