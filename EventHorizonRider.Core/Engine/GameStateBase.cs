using Microsoft.Xna.Framework;

namespace EventHorizonRider.Core.Engine
{
    internal abstract class GameStateBase
    {
        public abstract void Handle(GameContext gameContext, GameTime gameTime);
    }
}