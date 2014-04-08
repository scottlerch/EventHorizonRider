using System;
using Microsoft.Xna.Framework;

namespace EventHorizonRider.Core.Engine.States
{
    internal class MenuState : GameStateBase
    {
        public override void Handle(GameContext gameContext, GameTime gameTime)
        {
            gameContext.Root.Menu.Visible = true;

            gameContext.Root.Space.StartBlur(blurAmount:5f, speed:15f);
            gameContext.Root.Foreground.MenuButton.Show(true);
            gameContext.Root.Foreground.PlayButton.Hide(fade: true, fadeSpeed:10f);
            gameContext.Root.Space.Blackhole.SetExtraScale(3.5f, animate:true, speed:16f);
            gameContext.Root.Space.Background.Scale = gameContext.Root.Space.Blackhole.Scale.X;

            gameContext.Root.Menu.Visible = gameContext.Root.Space.Blackhole.ExtraScale == 3.5f;

            gameContext.Root.Menu.LevelSelect.MaximumStartLevel = gameContext.PlayerData.HighestLevelNumber;
            gameContext.Root.Menu.LevelSelect.StartLevel = gameContext.PlayerData.DefaultLevelNumber;

            var levelPressed = gameContext.Root.Menu.LevelSelect.Pressed;

            if (levelPressed.HasValue)
            {
                gameContext.IoTask = gameContext.PlayerData.UpdateDefaultLevel(levelPressed.Value);
            }
            else if (gameContext.Root.Menu.ResetButton.Pressed)
            {
                gameContext.IoTask = gameContext.PlayerData.Reset();
            }
            else if (gameContext.Root.Menu.CreditsButton.Pressed)
            {
                gameContext.Root.Menu.Credits.Visible = true;
                gameContext.Root.Menu.CreditsButton.Visible = false;
                gameContext.Root.Menu.ResetButton.Visible = false;
                gameContext.Root.Menu.LevelSelect.Visible = false;
            }
            else if (gameContext.Root.Foreground.MenuButton.Pressed)
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
                    gameContext.GameState = new InitializeState();
                }
            }
        }
    }
}
