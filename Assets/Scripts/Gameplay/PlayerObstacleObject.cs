using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObstacleObject : MonoBehaviour
{
    public Vector3 Speed { get; set; }

    void Awake()
    {
        Speed = Vector3.zero;
    }

    void Update()
    {
        // We make the obstacle move laterally at a constant speed
        transform.position = transform.position + Speed * Time.deltaTime;
    }
}
