using UnityEngine;

public class DesktopInput : DeviceInput
{
    protected override void SetInputDirection()
    {
        InputDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }
    protected override void SetTouchPosition()
    {
        isTouchingScreen = Input.GetMouseButton(0);
        if(isTouchingScreen)
        {
            TouchPosition = Input.mousePosition;
        }
    }
}
