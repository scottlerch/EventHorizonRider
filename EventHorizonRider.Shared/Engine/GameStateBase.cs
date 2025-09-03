using Microsoft.Xna.Framework;

namespace EventHorizonRider.Core.Engine;

/// <summary>
/// Base class for game states.  See concrete implementations in States namespace.
/// </summary>
internal abstract class GameStateBase
{
    private bool _beginning = true;

    public void Handle(GameContext gameContext, GameTime gameTime)
    {
        if (_beginning)
        {
            OnBegin(gameContext, gameTime);
            _beginning = false;
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
