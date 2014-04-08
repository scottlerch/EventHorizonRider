using System;
using Microsoft.Xna.Framework;

namespace EventHorizonRider.Core.Engine.States
{
    internal class InitializeState : GameStateBase
    {
        private readonly TimeSpan initInterval = TimeSpan.FromSeconds(2);

        public override void Handle(GameContext gameContext, GameTime gameTime)
        {
            gameContext.Root.Menu.Visible = false;

            gameContext.Root.Space.Background.Scale = 1f;
            gameContext.Root.Space.Blackhole.SetExtraScale(0f, animate:true, speed:16f);
            gameContext.Root.Space.Background.Scale = gameContext.Root.Space.Blackhole.Scale.X;
            gameContext.Root.Space.Background.Start();
            gameContext.Root.Space.Ship.Initialize();
            gameContext.Root.Space.Rings.Clear();
            gameContext.Root.Space.StopBlur();
            gameContext.Root.Space.Halo.Visible = true;
            gameContext.Root.Space.Ship.Initialize();

            gameContext.Root.Foreground.PlayTimer.HideLevelAndScore();
            gameContext.Root.Foreground.MenuButton.Show();
            gameContext.Root.Foreground.PlayButton.Scale = 1f;

            if (gameContext.Root.Space.Blackhole.ExtraScale == 0)
            {
                gameContext.Root.Foreground.PlayButton.Show(restart: false);
            }

            if (gameTime.TotalGameTime > initInterval)
            {
                gameContext.Root.Foreground.Title.FadingOut = true;

                if (!gameContext.Root.Foreground.Title.Visible)
                {
                    if (gameContext.Root.Foreground.PlayButton.Pressed)
                    {
                        gameContext.Root.Space.Blackhole.Pulse(1.5f, 2.5f);
                        gameContext.Root.Foreground.PlayButton.Hide();

                        gameContext.GameState = new StartingState();
                    }
                    else if (gameContext.Root.Foreground.MenuButton.Pressed)
                    {
                        gameContext.GameState = new MenuState();
                    }
                }
            }
        }
    }
}
