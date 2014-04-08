using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace EventHorizonRider.Core.Input
{
    internal class InputState
    {
        public KeyboardState KeyState;
        public TouchCollection TouchState;
        public MouseState MouseState;

        public void Update()
        {
            KeyState = Keyboard.GetState();
            TouchState = TouchPanel.GetState();
            
            // TODO: detect if device supports mouse, on iOS mouse gets values from touch which can mess things up
            // MouseState = Mouse.GetState();
        }
    }
}