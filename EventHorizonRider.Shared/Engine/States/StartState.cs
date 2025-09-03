using EventHorizonRider.Core.Components.ForegroundComponents;
using Microsoft.Xna.Framework;

namespace EventHorizonRider.Core.Engine.States;

internal class StartState : GameStateBase
{
    public override void OnBegin(GameContext gameContext, GameTime gameTime)
    {
        gameContext.Root.Menu.Visible = false;

        var defaultLevel = gameContext.Levels.GetLevel(gameContext.PlayerData.DefaultLevelNumber);
        gameContext.Root.Space.Color = defaultLevel.Color;
        gameContext.Root.Space.Ship.Color = defaultLevel.Color;
        gameContext.Root.Space.Ship.Shield.Color = defaultLevel.Color;
        gameContext.Root.Space.Background.StarBackgroundColor = defaultLevel.Color;
        gameContext.Root.Space.Background.RotationalVelocity = defaultLevel.RotationalVelocity;
        gameContext.Root.Space.Blackhole.RotationalVelocity = defaultLevel.RotationalVelocity;

        gameContext.Root.Space.Blackhole.SetExtraScale(0f, animate: true, speed: 16f);
        gameContext.Root.Space.Background.Start();
        gameContext.Root.Space.Ship.Initialize();
        gameContext.Root.Space.Rings.Clear();
        gameContext.Root.Space.StopBlur();
        gameContext.Root.Space.BlackholeHalo.Visible = true;
        gameContext.Root.Space.Ship.Speed = gameContext.Levels.CurrentLevel.ShipSpeed;

        gameContext.Root.Foreground.PlayButton.Scale = 1f;
        gameContext.Root.Foreground.ControlsHelp.Show(speed: 2f);
        gameContext.Root.Foreground.PlayTimer.HideLevelAndScore();
        gameContext.Root.Foreground.MenuButton.Show();
        gameContext.Root.Foreground.PlayButton.Show(
            state: PlayButtonState.Start,
            fade: gameContext.Root.Space.Blackhole.ExtraScale > 0f);

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
