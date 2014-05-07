using EventHorizonRider.Core.Components.ForegroundComponents;
using Microsoft.Xna.Framework;

namespace EventHorizonRider.Core.Engine.States
{
    internal class StartState : GameStateBase
    {
        public override void OnBegin(GameContext gameContext, GameTime gameTime)
        {
            gameContext.Root.Menu.Visible = false;

            gameContext.Root.Space.Background.StarBackgroundColor =
                gameContext.LevelCollection.GetLevel(gameContext.PlayerData.DefaultLevelNumber).Color;

            gameContext.Root.Space.Blackhole.SetExtraScale(0f, animate: true, speed: 16f);
            gameContext.Root.Space.Background.Start();
            gameContext.Root.Space.Ship.Initialize();
            gameContext.Root.Space.Rings.Clear();
            gameContext.Root.Space.StopBlur();
            gameContext.Root.Space.Halo.Visible = true;
            gameContext.Root.Space.Ship.Initialize();

            gameContext.Root.Foreground.PlayButton.Scale = 1f;
            gameContext.Root.Foreground.ControlsHelp.Show(speed: 2f);
            gameContext.Root.Foreground.PlayTimer.HideLevelAndScore();
            gameContext.Root.Foreground.MenuButton.Show();
            gameContext.Root.Foreground.PlayButton.Show(state: PlayButtonState.Start, fade: gameContext.Root.Space.Blackhole.ExtraScale > 0f);

            gameContext.Root.Foreground.PlayTimer.UpdateBest(gameContext.PlayerData.BestTime);
        }

        public override void OnProcess(GameContext gameContext, GameTime gameTime)
        {
            gameContext.Root.Space.Background.Scale = gameContext.Root.Space.Blackhole.Scale.X;

            if (!gameContext.Root.Foreground.Title.Visible)
            {
                if (gameContext.Root.Foreground.PlayButton.Button.Pressed)
                {
                    gameContext.GameState = new RunningState();
                }
                else if (gameContext.Root.Foreground.MenuButton.Button.Pressed)
                {
                    gameContext.GameState = new MenuState();
                }
            }
        }

        public override void OnEnd(GameContext gameContext, GameTime gameTime)
        {
        }
    }
}
