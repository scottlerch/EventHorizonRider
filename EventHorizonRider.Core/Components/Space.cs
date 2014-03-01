using EventHorizonRider.Core.Components.SpaceComponents;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Components
{
    internal class Space : ComponentBase
    {
        public Space(Background background, Halo halo, RingCollection ringCollection) : base(background, halo, ringCollection)
        {
        }

        protected override void OnBeforeDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
        }

        protected override void OnAfterDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
        }
    }
}
