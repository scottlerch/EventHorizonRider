using Microsoft.Xna.Framework;

namespace EventHorizonRider.Core.Engine.States
{
    internal class EndingState : GameStateBase
    {
        public override void Handle(GameContext gameContext, GameTime gameTime)
        {
            gameContext.Root.Space.Blur = true;
            gameContext.Root.Space.Halo.Visible = false;
            gameContext.Root.Space.Blackhole.SetExtraScale(0.3f);

            gameContext.Root.Music.Stop();

            gameContext.Root.Space.Background.Gameover();
            gameContext.Root.Space.Blackhole.Stop();
            gameContext.Root.Space.Ship.Stop();
            gameContext.Root.Space.Rings.Stop();

            gameContext.Root.Foreground.PlayTimer.Stop();
            gameContext.Root.Foreground.PlayButton.Show(true);

            gameContext.IoTask = gameContext.PlayerData.UpdateBestTime(gameContext.Root.Foreground.PlayTimer.Elapsed, gameContext.CurrentLevelNumber);

            gameContext.GameState = new OverState();
        }
    }
}
