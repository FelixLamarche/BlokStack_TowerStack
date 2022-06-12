using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        BlockTowerElement block; 

        if(other.TryGetComponent<BlockTowerElement>(out block))
        {
            block.CollisionWithSpike(this);
        }
    }
}
