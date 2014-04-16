using System;
using System.Linq;
using Microsoft.Xna.Framework;
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
            TouchState = new TouchCollection(
                TouchPanel.GetState().Select(touch => 
                    new TouchLocation(
                        touch.Id, 
                        touch.State, 
                        new Vector2(
                            touch.Position.X * ScreenInfo.InputScale, 
                            touch.Position.Y * ScreenInfo.InputScale))).ToArray());

            // TODO: detect if device supports mouse, on iOS mouse gets values from touch which can mess things up
            // MouseState = Mouse.GetState();
        }
    }
}