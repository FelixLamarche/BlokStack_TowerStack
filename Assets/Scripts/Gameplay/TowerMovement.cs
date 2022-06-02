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
        // TODO slow down the movement speed of touch controls
        // if(useTouchControls)
        // {
        //     Vector3 worldPoint = Camera.main.ScreenToWorldPoint(GameInput.TouchPosition + Vector3.back * Camera.main.transform.position.z);
        //     float nextXPosition = worldPoint.x - transform.position.x;
        //     if(Mathf.Abs(nextXPosition) > Mathf.Epsilon)
        //         Move(nextXPosition);
        //     else
        //         Decelerate();
        // }
        // else
        // {
        //     float inputLateralSpeed = GameInput.InputDirection.x;
        //     if(Mathf.Abs(inputLateralSpeed) > Mathf.Epsilon)
        //         Move(inputLateralSpeed);
        //     else 
        //         Decelerate();
        // }


        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(GameInput.TouchPosition);

        float nextXPosition = worldPoint.x - movingBlock.transform.position.x;
        Debug.Log($"Viewport: {Camera.main.ScreenToViewportPoint(GameInput.TouchPosition)}");
        Debug.Log($"WorldPoint: {worldPoint}");
        Debug.Log($"TouchPosition: ${GameInput.TouchPosition}");
        Debug.Log($"nextXPosition {nextXPosition}");

        float inputLateralSpeed = GameInput.InputDirection.x;

        if(Mathf.Abs(nextXPosition) > Mathf.Epsilon)
            Move(nextXPosition);
        else if(Mathf.Abs(inputLateralSpeed) > Mathf.Epsilon)
            Move(inputLateralSpeed);
        else 
            Decelerate();
    }

    // Used by game canvas button
    public void SetTouchControls(bool useTouchControls)
    {
        this.useTouchControls = useTouchControls;
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
