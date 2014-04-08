using EventHorizonRider.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System.Linq;

namespace EventHorizonRider.Core.Components
{
    internal class Button
    {
        public Rectangle ButtonBounds { get; private set; }

        public Keys? Key { get; private set; }

        public bool Pressed { get; private set; }

        public bool Hover { get; private set; }

        public Button(Rectangle buttonBounds, Keys? key = null)
        {
            ButtonBounds = buttonBounds;
            Key = key;
        }

        public void Update(InputState inputState, bool visible)
        {
            if (!visible)
            {
                Pressed = false;
                Hover = false;
                return;
            }

            Pressed = IsPressed(inputState.MouseState, inputState.TouchState, inputState.KeyState);
            Hover = IsHover(inputState.MouseState, inputState.TouchState, inputState.KeyState);
        }

        private bool IsPressed(MouseState mouseState, TouchCollection touchState, KeyboardState keyboardState)
        {
            return
                (Key.HasValue && keyboardState.GetPressedKeys().Contains(Key.Value)) ||
                touchState.Any(t => t.State == TouchLocationState.Released && ButtonBounds.Contains(t.Position)) ||
                (mouseState.LeftButton == ButtonState.Released && ButtonBounds.Contains(mouseState.Position));
        }

        private bool IsHover(MouseState mouseState, TouchCollection touchState, KeyboardState keyboardState)
        {
            return
                (Key.HasValue && keyboardState.GetPressedKeys().Contains(Key.Value)) ||
                touchState.Any(t => (t.State == TouchLocationState.Pressed || t.State == TouchLocationState.Moved) && ButtonBounds.Contains(t.Position)) ||
                (mouseState.LeftButton == ButtonState.Pressed && ButtonBounds.Contains(mouseState.Position));
        }
    }
}
