using Microsoft.Xna.Framework;

namespace EventHorizonRider.Core.Engine.States
{
    internal class InitializeState : GameStateBase
    {
        public override void Handle(GameContext gameContext, GameTime gameTime)
        {
            gameContext.Ship.Initialize();

            if (gameContext.PlayButton.Pressed)
            {
                gameContext.Blackhole.Pulse(1.5f, 2.5f);
                gameContext.PlayButton.Hide();

                gameContext.GameState = new StartingState();
            }
        }
    }
}
