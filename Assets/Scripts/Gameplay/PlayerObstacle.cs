using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObstacle : MonoBehaviour
{
    [SerializeField]
    Vector3 speed = Vector3.zero;

    void Update()
    {
        // We make the obstacle move laterally at a constant speed
        transform.position = transform.position + speed * Time.deltaTime;
    }
}
