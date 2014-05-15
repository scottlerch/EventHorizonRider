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

			var currentTouchState = TouchPanel.GetState();
			var scaledTouchLocations = new TouchLocation[currentTouchState.Count];
			
			for (int i = 0; i < currentTouchState.Count; i++)
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
}