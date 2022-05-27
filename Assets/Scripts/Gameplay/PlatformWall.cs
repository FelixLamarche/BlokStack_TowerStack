using UnityEngine;

public class PlatformWall : MonoBehaviour
{
    [SerializeField]
    Direction side;

    TowerMovement towerMovement;

    void Awake()
    {
        towerMovement = FindObjectOfType<TowerMovement>();
    }

    // Move walls to restrain the platform to within the screen but with half of the platform which can go out of the screen
    public void MoveToNewX(float distanceFromOrigin)
    {
        float newDeltaX = side == Direction.Left ? -distanceFromOrigin : distanceFromOrigin;
        transform.position = new Vector3(newDeltaX, transform.position.y, transform.position.z);
        if(side == Direction.Left)
            towerMovement?.SetBoundariesMovement(side, transform.position.x + transform.lossyScale.x / 2);
        else if(side == Direction.Right)
            towerMovement?.SetBoundariesMovement(side, transform.position.x - transform.lossyScale.x / 2);
    }
}

public enum Direction
{
    Left, Right
}
