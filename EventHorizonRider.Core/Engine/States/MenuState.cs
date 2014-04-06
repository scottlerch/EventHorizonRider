using Microsoft.Xna.Framework;

namespace EventHorizonRider.Core.Engine.States
{
    internal class MenuState : GameStateBase
    {
        public override void Handle(GameContext gameContext, GameTime gameTime)
        {
            gameContext.Space.Blur = true;
            gameContext.MenuButton.Show(true);
            gameContext.PlayButton.Hide(fade:false);
            gameContext.Blackhole.SetExtraScale(3.5f);
            gameContext.Background.Scale = gameContext.Blackhole.Scale.X;

            if (gameContext.MenuButton.Pressed)
            {
                gameContext.GameState = new InitializeState();
            }
        }
    }
}
