using EventHorizonRider.Core.Components.CenterComponents;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Components
{
    internal class Center : ComponentBase
    {
        public Center(Blackhole blackhole, PlayButton playButton, Ship ship) : base(blackhole, playButton, ship)
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
