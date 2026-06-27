using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace EventHorizonRider.Core.Input;

/// <summary>
/// Keeps track of input state scaled for current device.
/// </summary>
internal class InputState
{
    private static readonly TouchCollection EmptyTouchState = new([]);

    public KeyboardState KeyState;
    public TouchCollection TouchState;
    public MouseState MouseState;

    public void Update()
    {
        KeyState = Keyboard.GetState();

        var currentTouchState = TouchPanel.GetState();
        var touchCount = currentTouchState.Count;

        if (touchCount == 0)
        {
            // Avoid allocating a fresh (often empty) array every frame; this is the common case on desktop.
            TouchState = EmptyTouchState;
        }
        else
        {
            var scaledTouchLocations = new TouchLocation[touchCount];

            for (var i = 0; i < touchCount; i++)
            {
                var touch = currentTouchState[i];
                scaledTouchLocations[i] = new TouchLocation(
                    touch.Id,
                    touch.State,
                    new Vector2(
                        touch.Position.X * DeviceInfo.InputScale,
                        touch.Position.Y * DeviceInfo.InputScale));
            }

            TouchState = new TouchCollection(scaledTouchLocations);
        }

        var currentMouseState = Mouse.GetState();
        var scaledMouseState = new MouseState(
            (int)(currentMouseState.X * DeviceInfo.InputScale),
            (int)(currentMouseState.Y * DeviceInfo.InputScale),
            currentMouseState.ScrollWheelValue,
            currentMouseState.LeftButton,
            currentMouseState.MiddleButton,
            currentMouseState.RightButton,
            currentMouseState.XButton1,
            currentMouseState.XButton2);

        MouseState = scaledMouseState;
    }
}
