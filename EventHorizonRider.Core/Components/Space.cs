using EventHorizonRider.Core.Components.SpaceComponents;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Components
{
    internal class Space : ComponentBase
    {
        public Space(Background background, RingCollection ringCollection) : base(background, ringCollection)
        {
        }

        protected override void OnBeforeDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.Opaque);
        }

        protected override void OnAfterDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
        }
    }
}
