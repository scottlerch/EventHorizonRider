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

            // TODO: detect if device supports mouse, on iOS mouse gets values from touch which can mess things up
            // MouseState = Mouse.GetState();
        }
    }
}