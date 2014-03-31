using Microsoft.Xna.Framework;
using System;

namespace EventHorizonRider.Core.Engine.States
{
    internal class RunningState : GameStateBase
    {
        private readonly TimeSpan waitBetweenLevels = TimeSpan.FromSeconds(2);
        private TimeSpan levelEndTime = TimeSpan.Zero;
        private bool levelEnded;

        public override void Handle(GameContext gameContext, GameTime gameTime)
        {
            if (gameContext.Root.OverrideLevel.HasValue)
            {
                gameContext.CurrentLevelNumber = gameContext.Root.OverrideLevel.Value;

                gameContext.Rings.Clear();
                gameContext.PlayTimer.SetLevel(gameContext.CurrentLevelNumber);
                gameContext.Rings.SetLevel(gameContext.Levels.GetLevel(gameContext.CurrentLevelNumber));

                gameContext.Root.OverrideLevel = null;
            }
            else
            {
                if (gameContext.Rings.Intersects(gameContext.Ship))
                {
                    gameContext.GameState = new EndingState();
                }
                else if (!gameContext.Rings.HasMoreRings && gameContext.Rings.Children.Count == 0)
                {
                    if (!levelEnded)
                    {
                        levelEndTime = gameTime.TotalGameTime + waitBetweenLevels;
                        levelEnded = true;
                    }
                    else if (levelEndTime.Ticks < gameTime.TotalGameTime.Ticks)
                    {
                        levelEnded = false;
                        gameContext.CurrentLevelNumber++;

                        gameContext.PlayTimer.SetLevel(gameContext.CurrentLevelNumber);
                        gameContext.Rings.SetLevel(gameContext.Levels.GetLevel(gameContext.CurrentLevelNumber));
                    }
                }
            }
        }
    }
}
