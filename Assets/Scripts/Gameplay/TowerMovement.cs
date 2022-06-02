using System;
using UnityEngine;

public class TowerMovement : MonoBehaviour
{
    [SerializeField, MinAttribute(0f)]
    float maxSpeed = 1.5f;

    [SerializeField, MinAttribute(0.00001f), Tooltip("Time it takes to go from 0 to max Speed")]
    float accelerationTime = 0.5f;


    float leftSideMinX = Mathf.NegativeInfinity;
    float rightSideMaxX = Mathf.Infinity;

    float curSpeed = 0f;

    BlockPlatform platform;
    BlockTowerElement movingBlock;

    void Awake()
    {
        platform = FindObjectOfType<BlockPlatform>();
    }

    void Start()
    {
        ResetPlatform();
    }

    void FixedUpdate()
    {
        Vector3 worldPointTouch = Camera.main.ScreenToWorldPoint(GameInput.TouchPosition);
        float nextXPositionTouch = worldPointTouch.x - movingBlock.transform.position.x;

        float inputLateralSpeed = GameInput.InputDirection.x;

        if(GameInput.IsTouchingScreen && Mathf.Abs(nextXPositionTouch) > Mathf.Epsilon)
        {

            Debug.Log($"xPos: ${nextXPositionTouch}");
            float speedDirection = Mathf.Sign(nextXPositionTouch);
            // Normalize speed to be between [-1,1]
            float moveSpeedNormalized = Mathf.Abs(nextXPositionTouch) > 1 ? speedDirection : nextXPositionTouch;

            Move(moveSpeedNormalized);
        }
        else if(Mathf.Abs(inputLateralSpeed) > Mathf.Epsilon)
            Move(inputLateralSpeed);
        else 
            Decelerate();
    }

    public void ResetPlatform()
    {
        curSpeed = 0;
        movingBlock = platform;
    }

    public void SetMovingBlock(BlockTowerElement block)
    {
        movingBlock = block;
        if(block is FallingBlock)
        {
            ((FallingBlock) block).followBlockBelow = false;
        }
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
        float t = Time.deltaTime / accelerationTime + curSpeed / maxSpeed;
        curSpeed = Mathf.Lerp(0, maxSpeed, t);
        Vector3 newPos = movingBlock.transform.position + Vector3.right * normalizedSpeed * curSpeed * Time.deltaTime;
        // Clamping within the boundaries of the walls
        newPos.x = Mathf.Clamp(newPos.x, leftSideMinX, rightSideMaxX);
        movingBlock.GetComponent<Rigidbody2D>().MovePosition(newPos);
    }

    void Decelerate()
    {
        curSpeed = Mathf.Max(0, curSpeed - maxSpeed * Time.deltaTime / accelerationTime);
    }
}
