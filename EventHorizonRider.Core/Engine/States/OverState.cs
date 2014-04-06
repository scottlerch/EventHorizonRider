using Microsoft.Xna.Framework;

namespace EventHorizonRider.Core.Engine.States
{
    internal class OverState : GameStateBase
    {
        public override void Handle(GameContext gameContext, GameTime gameTime)
        {
            if (gameContext.PlayButton.Pressed)
            {
                gameContext.GameState = new InitializeState();
            }
        }
    }
}
