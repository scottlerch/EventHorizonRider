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

        public Space Space { get; private set; }

        public Menu Menu { get; private set; }

        public Foreground Foreground { get; private set; }

        public Music Music { get; private set; }

        public Root(Music music, Space space, Menu menu, Foreground foreground)
            : base(music, space, menu, foreground)
        {
            Music = music;
            Space = space;
            Menu = menu;
            Foreground = foreground;
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
