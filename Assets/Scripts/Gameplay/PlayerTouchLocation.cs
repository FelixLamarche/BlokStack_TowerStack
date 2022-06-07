using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTouchLocation : MonoBehaviour
{
    GameCamera gameCamera;
    CircleCollider2D touchCollider;
    TrailRenderer trail;

    void Awake()
    {
        gameCamera = FindObjectOfType<GameCamera>();

        touchCollider = GetComponent<CircleCollider2D>();
        trail = GetComponentInChildren<TrailRenderer>();
    }

    void Update(){
        touchCollider.enabled = GameInput.IsTouchingScreen;
        trail.enabled = GameInput.IsTouchingScreen;
        
        if(GameInput.IsTouchingScreen)
        {
            Vector3 screenPosWithDepth = new Vector3(GameInput.TouchPosition.x, GameInput.TouchPosition.y, transform.position.z);
            Vector2 pos2D = gameCamera.ScreenPositionToWorldPoint2D(screenPosWithDepth);
            transform.position = new Vector3(pos2D.x, pos2D.y, transform.position.z);
        }
    }

    void OnTriggerEnter2D(Collider2D collider2D){
        if(collider2D.tag == "PlayerObstacle"){
            FindObjectOfType<GameplayManager>().StopGame();
        }
    }

    void OnDrawGizmos()
    {
        float radius = touchCollider == null ? 0.25f : touchCollider.radius;
        radius = Mathf.Max(radius, 0.15f); // Make it more visible if radius is too small
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
