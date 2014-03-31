using System.Linq;
using EventHorizonRider.Core.Extensions;
using EventHorizonRider.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EventHorizonRider.Core.Components
{
    internal class Root : ComponentBase
    {
        public int? OverrideLevel { get; set; }

        public Root(Music music, Space space, Foreground foreground)
            : base(music, space, foreground)
        {
        }

        protected override void UpdateCore(GameTime gameTime, InputState inputState)
        {
            var keys = inputState.KeyState.GetPressedKeys();

            foreach (var key in keys)
            {
                if (key >= Keys.D0 && key <= Keys.D9)
                {
                    OverrideLevel = (int)key - (int)Keys.D0;
                }
            }
        }
    }
}
