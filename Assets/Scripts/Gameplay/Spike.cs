using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.TryGetComponent<BlockTowerElement>(out BlockTowerElement block) && block.IsTopBlock())
        {
            block.CollisionWithSpike(this);
        }
    }
}
