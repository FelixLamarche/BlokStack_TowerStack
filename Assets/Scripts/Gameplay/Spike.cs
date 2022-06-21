using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision2D)
    {
        //Debug.Log(collision2D.GetContact(0).normal);

        if(collision2D.gameObject.TryGetComponent(out BlockTowerElement block) && 
            block.IsTopBlock() &&
            collision2D.GetContact(0).normal.y > 0.5f) // Collision must be head-on for the spike to end the game
        {
            block.CollisionWithSpike(this);
        }
    }
}
