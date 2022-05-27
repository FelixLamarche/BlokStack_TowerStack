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
    public static Vector3 TouchPosition {
        get {return deviceInput.TouchPosition;}
    }
    static Vector2 inputDirection;
    static DeviceInput deviceInput;

    static GameInput()
    {
        inputDirection = Vector2.zero;
        CreateInputDevice();
    }

    static void CreateInputDevice()
    {
        if(!(deviceInput is null))
            return;

        deviceInput = GameManager.instance.AddDeviceInput();
    }

    public static void ResetInput() {
        deviceInput?.ResetInput();
    }
}
