using UnityEngine;

public class MobileInput : DeviceInput
{
    protected override void SetInputDirection()
    {
        InputDirection = (Vector2) Input.acceleration;
    }

    protected override void SetTouchPosition()
    {
        IsTouchingScreen = Input.touchCount > 0;
        if(IsTouchingScreen)
        {
            TouchPosition = Input.GetTouch(0).position;
        }
    }
}
