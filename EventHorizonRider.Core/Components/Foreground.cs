using EventHorizonRider.Core.Components.ForegroundComponents;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Components
{
    internal class Foreground : ComponentBase
    {
        public Foreground(PlayButton playButton, PlayTimer playTime, Title title, FpsCounter fpsCounter) : base(playButton, playTime, title, fpsCounter)
        {
        }

        protected override void OnBeforeDraw(SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
        }

        protected override void OnAfterDraw(SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            spriteBatch.End();
        }
    }
}
