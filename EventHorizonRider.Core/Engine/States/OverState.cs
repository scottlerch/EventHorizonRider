using Microsoft.Xna.Framework;

namespace EventHorizonRider.Core.Engine.States
{
    internal class OverState : GameStateBase
    {
        public override void Handle(GameContext gameContext, GameTime gameTime)
        {
            if (gameContext.Root.Foreground.PlayButton.Button.Pressed)
            {
                gameContext.Root.Space.Blackhole.SetExtraScale(0f);

                gameContext.GameState = new InitializeState();
            }
        }
    }
}
