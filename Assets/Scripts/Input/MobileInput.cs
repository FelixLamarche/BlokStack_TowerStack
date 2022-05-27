using UnityEngine;

public class MobileInput : DeviceInput
{
    protected override void SetInputDirection()
    {
        InputDirection = (Vector2) Input.acceleration;
    }

    protected override void SetTouchPosition()
    {
        if(Input.touchCount == 0)
            return;
            
        TouchPosition = Input.GetTouch(0).position;
    }
}
