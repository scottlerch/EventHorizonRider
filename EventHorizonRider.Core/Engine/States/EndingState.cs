using Microsoft.Xna.Framework;

namespace EventHorizonRider.Core.Engine.States
{
    internal class EndingState : GameStateBase
    {
        public override void Handle(GameContext gameContext, GameTime gameTime)
        {
            gameContext.Space.Blur = true;
            gameContext.Halo.Visible = false;
            gameContext.Blackhole.SetSize(1.2f);

            gameContext.Music.Stop();

            gameContext.Background.Gameover();
            gameContext.Blackhole.Stop();
            gameContext.Ship.Stop();
            gameContext.Rings.Stop();

            gameContext.PlayTimer.Stop();
            gameContext.PlayButton.Show(true);

            gameContext.PlayerData.UpdateBestTime(gameContext.PlayTimer.Elapsed);

            gameContext.GameState = new OverState();
        }
    }
}
