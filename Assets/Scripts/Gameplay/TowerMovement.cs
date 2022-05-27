using System;
using UnityEngine;

public class TowerMovement : MonoBehaviour
{
    [SerializeField]
    bool useTouchControls = false;

    [SerializeField, MinAttribute(0f)]
    float maxSpeed = 1.5f;

    [SerializeField, MinAttribute(0.00001f), Tooltip("Time it takes to go from 0 to max Speed")]
    float accelerationTime = 0.5f;


    float leftSideMinX = Mathf.NegativeInfinity;
    float rightSideMaxX = Mathf.Infinity;

    float speed = 0f;

    PlatformTower platform;
    BlockTowerElement movingBlock;

    void Awake()
    {
        platform = FindObjectOfType<PlatformTower>();
    }

    void Start()
    {
        ResetPlatform();
    }

    void FixedUpdate()
    {
        // TODO slow down the movement speed of touch controls
        if(useTouchControls)
        {
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(GameInput.TouchPosition + Vector3.back * Camera.main.transform.position.z);
            float nextXPosition = worldPoint.x - transform.position.x;
            if(Mathf.Abs(nextXPosition) > Mathf.Epsilon)
                Move(nextXPosition);
            else
                Decelerate();
        }
        else
        {
            float inputLateralSpeed = GameInput.InputDirection.x;
            if(Mathf.Abs(inputLateralSpeed) > Mathf.Epsilon)
                Move(inputLateralSpeed);
            else 
                Decelerate();
        }
    }

    // Used by game canvas button
    public void SetTouchControls(bool useTouchControls)
    {
        this.useTouchControls = useTouchControls;
    }

    public void ResetPlatform()
    {
        speed = 0;
        movingBlock = platform;
    }

    public void SetMovingBlock(BlockTowerElement block)
    {
        movingBlock = block;
        if(block is BlockNoPhysics)
            ((BlockNoPhysics) block).followBlockBelow = false;
        block.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }

    public void SetBoundariesMovement(Direction side, float wallXPos)
    {
        if(platform == null)
            return;

        if(side == Direction.Left)
            leftSideMinX = wallXPos + platform.GetComponent<BoxCollider2D>().size.x / 2;
        else if(side == Direction.Right)
            rightSideMaxX = wallXPos - platform.GetComponent<BoxCollider2D>().size.x / 2;
    }


    // normalizedSpeed : [-1, 1]
    void Move(float normalizedSpeed)
    {
        float t = Time.deltaTime / accelerationTime + speed / maxSpeed;
        speed = Mathf.Lerp(0, maxSpeed, t);
        Vector3 newPos = movingBlock.transform.position + Vector3.right * normalizedSpeed * speed * Time.deltaTime;
        // Clamping within the boundaries of the walls
        newPos.x = Mathf.Clamp(newPos.x, leftSideMinX, rightSideMaxX);
        movingBlock.GetComponent<Rigidbody2D>().MovePosition(newPos);
    }

    void Decelerate()
    {
        speed = Mathf.Max(0, speed - maxSpeed * Time.deltaTime / accelerationTime);
    }
}
