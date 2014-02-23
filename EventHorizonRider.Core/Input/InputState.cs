using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace EventHorizonRider.Core.Input
{
    internal class InputState
    {
        public KeyboardState KeyState = Keyboard.GetState();
        public MouseState MouseState = Mouse.GetState();
        public TouchCollection TouchState = TouchPanel.GetState();

        public void Update()
        {
            KeyState = Keyboard.GetState();
            MouseState = Mouse.GetState();
            TouchState = TouchPanel.GetState();
        }
    }
}