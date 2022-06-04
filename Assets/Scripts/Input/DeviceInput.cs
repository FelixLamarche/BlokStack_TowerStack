using System;
using UnityEngine;

public abstract class DeviceInput : MonoBehaviour
{
    public bool AcceptInputs 
    {
        get {return acceptInputs;}
        set {acceptInputs = value;}
    }
    public Vector2 InputDirection
    {
        get {return inputDirection;}
        protected set {
            if(value != inputDirection && acceptInputs)
            {
                inputDirection = value;
                OnInputDirectionChanged();
            }
        }
    }
    public EventHandler<InputEventArgs> onInputDirectionChanged;

    public Vector2 TouchPosition
    {
        get {return touchPosition;}
        protected set {
            if(value != touchPosition && acceptInputs)
            {
                touchPosition = value;
            }
        }
    }

    public bool isTouchingScreen {get; protected set;}

    protected bool acceptInputs = false;
    private Vector2 inputDirection = Vector2.zero;
    private Vector2 touchPosition = Vector2.zero;


    void Update()
    {
        SetInputDirection();
        SetTouchPosition();
    }
    
    public void ResetInput()
    {
        bool oldAcceptInputs = acceptInputs;
        acceptInputs = true;

        InputDirection = Vector2.zero;
        TouchPosition = new Vector2(Camera.main.scaledPixelWidth / 2, Camera.main.scaledPixelHeight / 2);

        acceptInputs = oldAcceptInputs;
    }

    protected abstract void SetInputDirection();
    protected abstract void SetTouchPosition();
    
    protected void OnInputDirectionChanged() => onInputDirectionChanged?.Invoke(this, new InputEventArgs(InputDirection));
}

public class InputEventArgs : EventArgs
{
    public readonly Vector2 inputDirection;

    public InputEventArgs(Vector2 inputDirection)
    {
        this.inputDirection = inputDirection;
    }
}
