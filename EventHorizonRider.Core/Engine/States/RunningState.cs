using Microsoft.Xna.Framework;
using System;

namespace EventHorizonRider.Core.Engine.States
{
    internal class RunningState : GameStateBase
    {
        private readonly TimeSpan waitBetweenLevels = TimeSpan.FromSeconds(0.1);
        private TimeSpan levelEndTime = TimeSpan.Zero;
        private bool levelEnded;

        private void UpdateLevel(GameContext gameContext)
        {
            var level = gameContext.Levels.GetLevel(gameContext.CurrentLevelNumber);
            gameContext.Root.Foreground.PlayTimer.SetLevel(gameContext.CurrentLevelNumber);
            gameContext.Root.Space.Rings.SetLevel(level);
            gameContext.Root.Space.Ship.Speed = level.ShipSpeed;
        }

        public override void Handle(GameContext gameContext, GameTime gameTime)
        {
            gameContext.Root.Foreground.PlayButton.Scale = gameContext.Root.Space.Blackhole.Scale.X;

            if (gameContext.Root.OverrideLevel.HasValue)
            {
                gameContext.CurrentLevelNumber = gameContext.Root.OverrideLevel.Value;

                gameContext.Root.Space.Rings.Clear();
                
                UpdateLevel(gameContext);

                gameContext.Root.OverrideLevel = null;
            }
            else
            {
                if (gameContext.Root.Space.Rings.Intersects(gameContext.Root.Space.Ship))
                {
                    gameContext.GameState = new EndingState();
                }
                else if (!gameContext.Root.Space.Rings.HasMoreRings && gameContext.Root.Space.Rings.ChildrenCount == 0)
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

                        UpdateLevel(gameContext);
                    }
                }
            }
        }
    }
}
