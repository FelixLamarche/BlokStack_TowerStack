using UnityEngine;

public class BlockDetector : MonoBehaviour
{
    [SerializeField, MinAttribute(0f)]
    float verticalDistanceFromBottomOfCamera;

    GameCamera gameCamera;
    GameplayManager gameplayManager;

    void Awake()
    {
        gameCamera = FindObjectOfType<GameCamera>();
        gameplayManager = FindObjectOfType<GameplayManager>();
    }

    void LateUpdate()
    {
        FollowCamera();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        CheckForBlockCollision(collider);
    }

    void CheckForBlockCollision(Collider2D collider) 
    {
        if(collider.TryGetComponent(out BlockTowerElement blockElement) && 
            blockElement.TowerIn is null)
        {
            gameplayManager.StopGame();
        }
    }

    void FollowCamera()
    {
        float newY = gameCamera.CalculateVerticalEdgeOfScreen(transform.position.z, VerticalDirection.below);
        transform.position = new Vector3(transform.position.x, newY - verticalDistanceFromBottomOfCamera, transform.position.z);
    }
}
