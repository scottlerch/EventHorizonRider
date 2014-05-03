using Microsoft.Xna.Framework;

namespace EventHorizonRider.Core.Engine
{
    internal abstract class GameStateBase
    {
        private bool beginning = true;

        public void Handle(GameContext gameContext, GameTime gameTime)
        {
            if (beginning)
            {
                OnBegin(gameContext, gameTime);
                beginning = false;
            }
            else
            {
                OnProcess(gameContext, gameTime);

                if (gameContext.GameState != this)
                {
                    OnEnd(gameContext, gameTime);
                }
            }
        }

        public abstract void OnBegin(GameContext gameContext, GameTime gameTime);

        public abstract void OnProcess(GameContext gameContext, GameTime gameTime);

        public abstract void OnEnd(GameContext gameContext, GameTime gameTime);
    }
}