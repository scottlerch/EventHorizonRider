using System;
using Microsoft.Xna.Framework;

namespace EventHorizonRider.Core.Engine.States;

internal class InitializeState : GameStateBase
{
    private readonly TimeSpan initInterval = TimeSpan.FromSeconds(2);

    public override void OnBegin(GameContext gameContext, GameTime gameTime)
    {
        gameContext.Root.Foreground.Title.Show();
    }

    public override void OnProcess(GameContext gameContext, GameTime gameTime)
    {
        if (gameTime.TotalGameTime > initInterval)
        {
            gameContext.GameState = new StartState();
        }
    }

    public override void OnEnd(GameContext gameContext, GameTime gameTime)
    {
        gameContext.Root.Foreground.Title.Hide();
    }
}
