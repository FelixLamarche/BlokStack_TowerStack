using UnityEngine;

// Proxy Global class to represent game Inputs
public static class GameInput
{
    public static bool AcceptInputs {
        get {return deviceInput.AcceptInputs;}
        set {deviceInput.AcceptInputs = value;}
    }
    public static Vector2 InputDirection {
        get {return deviceInput.InputDirection;}
    }
    /// <summary>
    /// Position in pixels with (0,0) being positioned on the bottom left
    /// </summary>
    public static Vector2 TouchPosition {
        get {return deviceInput.TouchPosition;}
    }
    public static bool IsTouchingScreen {
        get {return deviceInput.IsTouchingScreen;}
    }
    static DeviceInput deviceInput;

    static GameInput()
    {
        CreateInputDevice();
    }

    static void CreateInputDevice()
    {
        if(!(deviceInput is null))
            return;

        deviceInput = GameManager.Instance.AddDeviceInput();
    }

    public static void ResetInput() 
    {
        if(deviceInput != null)
            deviceInput.ResetInput();
    }
}
