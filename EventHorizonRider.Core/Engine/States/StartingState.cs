using Microsoft.Xna.Framework;

namespace EventHorizonRider.Core.Engine.States
{
    internal class StartingState : GameStateBase
    {
        public override void Handle(GameContext gameContext, GameTime gameTime)
        {
            gameContext.Space.Blur = false;
            gameContext.Halo.Visible = true;

            gameContext.CurrentLevelNumber = 1;

            gameContext.PlayTimer.SetLevel(gameContext.CurrentLevelNumber);
            gameContext.PlayTimer.Restart();

            gameContext.Blackhole.Start();
            gameContext.Ship.Start();
            gameContext.Rings.Start();
            gameContext.Background.Start();

            gameContext.Rings.SetLevel(gameContext.Levels.GetLevel(gameContext.CurrentLevelNumber));

            gameContext.Music.Play();

            gameContext.GameState = new RunningState();
        }
    }
}
