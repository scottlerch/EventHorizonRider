using System;
using Microsoft.Xna.Framework;

namespace EventHorizonRider.Core.Engine.States
{
    internal class InitializeState : GameStateBase
    {
        private TimeSpan initInterval = TimeSpan.FromSeconds(2);

        public override void Handle(GameContext gameContext, GameTime gameTime)
        {
            gameContext.Background.Scale = 1f;
            gameContext.Blackhole.SetExtraScale(0f);
            gameContext.Background.Start();
            gameContext.PlayTimer.HideLevelAndScore();
            gameContext.Ship.Initialize();
            gameContext.Rings.Clear();
            gameContext.Space.Blur = false;
            gameContext.Halo.Visible = true;
            gameContext.Ship.Initialize();
            gameContext.MenuButton.Show();
            gameContext.PlayButton.Show(restart:false);

            if (gameTime.TotalGameTime > initInterval)
            {
                gameContext.Title.FadingOut = true;

                if (!gameContext.Title.Visible)
                {
                    if (gameContext.PlayButton.Pressed)
                    {
                        gameContext.Blackhole.Pulse(1.5f, 2.5f);
                        gameContext.PlayButton.Hide();

                        gameContext.GameState = new StartingState();
                    }
                    else if (gameContext.MenuButton.Pressed)
                    {
                        gameContext.GameState = new MenuState();
                    }
                }
            }
        }
    }
}
