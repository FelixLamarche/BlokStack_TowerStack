using UnityEngine;

public class MobileInput : DeviceInput
{
    protected override void SetInputDirection()
    {
        InputDirection = (Vector2) Input.acceleration;
    }

    protected override void SetTouchPosition()
    {
        isTouchingScreen = Input.touchCount > 0;
        if(isTouchingScreen)
        {
            TouchPosition = Input.GetTouch(0).position;
        }
    }
}
