using Microsoft.Xna.Framework;

namespace EventHorizonRider.Core.Engine.States
{
    internal class StartingState : GameStateBase
    {
        public override void Handle(GameContext gameContext, GameTime gameTime)
        {
            gameContext.CurrentLevelNumber = gameContext.PlayerData.DefaultLevelNumber;

            gameContext.Root.Foreground.PlayTimer.SetLevel(gameContext.CurrentLevelNumber);
            gameContext.Root.Foreground.PlayTimer.Restart();
            gameContext.Root.Foreground.PlayTimer.ShowLevelAndScore();
            gameContext.Root.Foreground.MenuButton.Hide();

            gameContext.Root.Space.Blur = false;
            gameContext.Root.Space.Halo.Visible = true;
            gameContext.Root.Space.Blackhole.SetExtraScale(0f);

            gameContext.Root.Space.Blackhole.Start();
            gameContext.Root.Space.Ship.Start();
            gameContext.Root.Space.Rings.Start();
            gameContext.Root.Space.Background.Start();

            var level = gameContext.Levels.GetLevel(gameContext.CurrentLevelNumber);
            gameContext.Root.Space.Rings.SetLevel(level);
            gameContext.Root.Space.Ship.Speed = level.ShipSpeed;

            gameContext.Root.Music.Play();

            gameContext.GameState = new RunningState();
        }
    }
}
