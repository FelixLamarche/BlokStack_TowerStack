using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTouchLocation : MonoBehaviour
{
    [SerializeField]
    GameObject[] test;

    GameCamera gameCamera;
    CircleCollider2D touchCollider;

    void Awake()
    {
        gameCamera = FindObjectOfType<GameCamera>();
        touchCollider = GetComponent<CircleCollider2D>();
    }

    void Update(){
        touchCollider.enabled = GameInput.IsTouchingScreen;
        if(GameInput.IsTouchingScreen)
        {
            // TODO GO FIX Calculate EDGE OF SCREEN To ACCOUNT FOR TOP/BOTTOM DISCREPANCY
            float yTop = gameCamera.CalculateEdgeOfScreenPosition(0, VerticalDirection.above);
            float tBot = gameCamera.CalculateEdgeOfScreenPosition(0, VerticalDirection.below);
            Debug.Log("Top: " + yTop);
            Debug.Log("Bot: " + tBot);
            test[0].transform.position = new Vector3(0, yTop, 0);
            test[1].transform.position = new Vector3(0, tBot, 0);

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
        float radius = touchCollider == null ? 0.5f : touchCollider.radius;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
