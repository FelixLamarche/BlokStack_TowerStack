using UnityEngine;

public class DesktopInput : DeviceInput
{
    protected override void SetInputDirection()
    {
        InputDirection = new(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }
    protected override void SetTouchPosition()
    {
        IsTouchingScreen = Input.GetMouseButton(0);
        if(IsTouchingScreen)
        {
            TouchPosition = Input.mousePosition;
        }
    }
}
