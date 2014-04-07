using EventHorizonRider.Core.Components.ForegroundComponents;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Components
{
    internal class Foreground : ComponentBase
    {
        public FpsCounter FpsCounter { get; private set; }

        public PlayButton PlayButton { get; private set; }

        public MenuButton MenuButton { get; private set; }

        public PlayTimer PlayTimer { get; private set; }

        public Title Title { get; private set; }

        public Foreground(PlayButton playButton, MenuButton menuButton, PlayTimer playTime, Title title, FpsCounter fpsCounter) 
            : base(playButton, menuButton, playTime, title, fpsCounter)
        {
            PlayButton = playButton;
            MenuButton = menuButton;
            PlayTimer = playTime;
            Title = title;
            FpsCounter = fpsCounter;
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
