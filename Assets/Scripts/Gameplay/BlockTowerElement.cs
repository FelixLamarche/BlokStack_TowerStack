using UnityEngine;

public class BlockTowerElement : MonoBehaviour
{
    public BlockTower TowerIn {get; protected set;}
    public float Height {get; protected set;}
    public int Score {get; protected set;}
    public bool IsAttachedToTower{get; set;}

    public void CollisionWithSpike(Spike spikeCollided)
    {
        if(TowerIn == null || !IsAttachedToTower) return;

        TowerIn.CollisionWithSpike(spikeCollided);
    }

    public bool IsTopBlock()
    {
        if(TowerIn == null) return false;
        
        return TowerIn.IsTopOfTower(this);
    }
}
