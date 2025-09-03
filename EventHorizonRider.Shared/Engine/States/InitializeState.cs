using Microsoft.Xna.Framework;
using System;

namespace EventHorizonRider.Core.Engine.States;

internal class InitializeState : GameStateBase
{
    private readonly TimeSpan _initInterval = TimeSpan.FromSeconds(2);

    public override void OnBegin(GameContext gameContext, GameTime gameTime) => gameContext.Root.Foreground.Title.Show();

    public override void OnProcess(GameContext gameContext, GameTime gameTime)
    {
        if (gameTime.TotalGameTime > _initInterval)
        {
            gameContext.GameState = new StartState();
        }
    }

    public override void OnEnd(GameContext gameContext, GameTime gameTime) => gameContext.Root.Foreground.Title.Hide();
}
