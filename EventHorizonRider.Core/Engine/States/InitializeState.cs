using System;
using Microsoft.Xna.Framework;

namespace EventHorizonRider.Core.Engine.States
{
    internal class InitializeState : GameStateBase
    {
        private TimeSpan initInterval = TimeSpan.FromSeconds(2);

        public override void Handle(GameContext gameContext, GameTime gameTime)
        {
            gameContext.Halo.Visible = true;
            gameContext.Ship.Initialize();

            if (gameTime.TotalGameTime > initInterval)
            {
                gameContext.Title.FadingOut = true;

                if (!gameContext.Title.Visible && gameContext.PlayButton.Pressed)
                {
                    gameContext.Blackhole.Pulse(1.5f, 2.5f);
                    gameContext.PlayButton.Hide();

                    gameContext.GameState = new StartingState();
                }
            }
        }
    }
}
