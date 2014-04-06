using Microsoft.Xna.Framework;

namespace EventHorizonRider.Core.Engine.States
{
    internal class StartingState : GameStateBase
    {
        public override void Handle(GameContext gameContext, GameTime gameTime)
        {
            gameContext.PlayTimer.ShowLevelAndScore();
            gameContext.MenuButton.Hide();

            gameContext.Space.Blur = false;
            gameContext.Halo.Visible = true;
            gameContext.Blackhole.SetExtraScale(0f);

            gameContext.CurrentLevelNumber = 1;

            gameContext.PlayTimer.SetLevel(gameContext.CurrentLevelNumber);
            gameContext.PlayTimer.Restart();

            gameContext.Blackhole.Start();
            gameContext.Ship.Start();
            gameContext.Rings.Start();
            gameContext.Background.Start();

            var level = gameContext.Levels.GetLevel(gameContext.CurrentLevelNumber);
            gameContext.Rings.SetLevel(level);
            gameContext.Ship.Speed = level.ShipSpeed;

            gameContext.Music.Play();

            gameContext.GameState = new RunningState();
        }
    }
}
