using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    [SerializeField]
    Collider2D spikePointCollider;

    void OnCollisionEnter2D(Collision2D collision2D)
    {
        if (collision2D.otherCollider == spikePointCollider &&
            collision2D.gameObject.TryGetComponent(out BlockTowerElement block) &&
            collision2D.GetContact(0).normal.y > 0.5f) // Collision must be head-on for the spike to end the game
        {
            block.CollisionWithSpike(this);
        }
    }
}
