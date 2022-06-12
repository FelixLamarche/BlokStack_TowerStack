using UnityEngine;

public class BlockTowerElement : MonoBehaviour
{
    public BlockTower TowerIn {get; protected set;}
    public float Height {get; protected set;}
    public int Score {get; protected set;}

    // TODO Block should check when it is in the tower visibly or not

    public void CollisionWithSpike(Spike spikeCollided)
    {
        if(TowerIn == null) return;

        TowerIn.CollisionWithSpike(spikeCollided);
    }

}
