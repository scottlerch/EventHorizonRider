using EventHorizonRider.Core.Components.ForegroundComponents;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Components
{
    internal class Foreground : ComponentBase
    {
        public Foreground(PlayTimer playTime, FpsCounter fpsCounter) : base(playTime, fpsCounter)
        {
        }

        protected override void OnBeforeDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
        }

        protected override void OnAfterDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
        }
    }
}
