using System;
using Microsoft.Xna.Framework;

namespace EventHorizonRider.Core.Engine.States
{
    internal class MenuState : GameStateBase
    {
        public override void OnBegin(GameContext gameContext, GameTime gameTime)
        {
            gameContext.Root.Space.Blackhole.SetExtraScale(3.5f, animate: true, speed: 16f);
            gameContext.Root.Space.StartBlur(blurAmount: 5f, speed: 15f);

            gameContext.Root.Foreground.ControlsHelp.Hide(speed: 4f);
            gameContext.Root.Foreground.MenuButton.Show(true);
            gameContext.Root.Foreground.PlayButton.Hide(fade: true, newFadeSpeed: 10f);

            gameContext.Root.Menu.LevelSelect.MaximumStartLevel = gameContext.PlayerData.HighestLevelNumber;
            gameContext.Root.Menu.LevelSelect.StartLevel = gameContext.PlayerData.DefaultLevelNumber;
        }

        public override void OnProcess(GameContext gameContext, GameTime gameTime)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            gameContext.Root.Menu.Visible = gameContext.Root.Space.Blackhole.ExtraScale == 3.5f;
            gameContext.Root.Space.Background.Scale = gameContext.Root.Space.Blackhole.Scale.X;

            var levelPressed = gameContext.Root.Menu.LevelSelect.Pressed;

            if (levelPressed.HasValue)
            {
                gameContext.IoTask = gameContext.PlayerData.UpdateDefaultLevel(levelPressed.Value);
                gameContext.Root.Menu.LevelSelect.StartLevel = gameContext.PlayerData.DefaultLevelNumber;

                var defaultLevel = gameContext.LevelCollection.GetLevel(gameContext.PlayerData.DefaultLevelNumber);
                gameContext.Root.Space.Background.StarBackgroundColor = defaultLevel.Color;
                gameContext.Root.Space.Background.RotationalVelocity = defaultLevel.RotationalVelocity;
                gameContext.Root.Space.Blackhole.RotationalVelocity = defaultLevel.RotationalVelocity;
            }
            else if (gameContext.Root.Menu.ResetButton.Button.Pressed)
            {
                gameContext.IoTask = gameContext.PlayerData.Reset();
                gameContext.Root.Foreground.PlayTimer.UpdateBest(TimeSpan.Zero);
                gameContext.Root.Menu.LevelSelect.MaximumStartLevel = 1;
                gameContext.Root.Menu.LevelSelect.StartLevel = 1;
            }
            else if (gameContext.Root.Menu.CreditsButton.Button.Pressed)
            {
                gameContext.Root.Menu.Credits.Visible = true;
                gameContext.Root.Menu.CreditsButton.Visible = false;
                gameContext.Root.Menu.ResetButton.Visible = false;
                gameContext.Root.Menu.LevelSelect.Visible = false;
            }
            else if (gameContext.Root.Foreground.MenuButton.Button.Pressed)
            {
                if (gameContext.Root.Menu.Credits.Visible)
                {
                    gameContext.Root.Menu.Credits.Visible = false;
                    gameContext.Root.Menu.CreditsButton.Visible = true;
                    gameContext.Root.Menu.ResetButton.Visible = true;
                    gameContext.Root.Menu.LevelSelect.Visible = true;
                }
                else
                {
                    gameContext.GameState = new StartState();
                }
            }
        }

        public override void OnEnd(GameContext gameContext, GameTime gameTime)
        {
        }
    }
}
