using EventHorizonRider.Core.Components.ForegroundComponents;
using Microsoft.Xna.Framework;
using System;

namespace EventHorizonRider.Core.Engine.States
{
    internal class RunningState : GameStateBase
    {
        private readonly TimeSpan waitBetweenLevels = TimeSpan.FromSeconds(0.1);
        private TimeSpan levelEndTime = TimeSpan.Zero;
        private TimeSpan totalElapsedGameTime = TimeSpan.Zero;
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
            UpdatePauseState(gameContext);

            if (gameContext.Paused)
            {
                gameContext.Root.Music.Pause();
                gameContext.Root.Foreground.PlayButton.Show(state:PlayButtonState.Resume);
                gameContext.Root.Space.SetBlur(blurAmount: 1.5f);
                gameContext.Root.Space.Updating = true;
                gameContext.Root.Music.Updating = true;
                gameContext.Root.Foreground.PlayTimer.Updating = true;
                return;
            }

            gameContext.Root.Music.Play();
            gameContext.Root.Foreground.PlayButton.Show(state:PlayButtonState.Pause);
            gameContext.Root.Space.SetBlur(blurAmount: 0f);
            gameContext.Root.Space.Updating = false;
            gameContext.Root.Music.Updating = false;
            gameContext.Root.Foreground.PlayTimer.Updating = false;

            totalElapsedGameTime += gameTime.ElapsedGameTime;
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
                        levelEndTime = totalElapsedGameTime + waitBetweenLevels;
                        levelEnded = true;
                    }
                    else if (levelEndTime.Ticks < totalElapsedGameTime.Ticks)
                    {
                        levelEnded = false;
                        gameContext.CurrentLevelNumber++;

                        UpdateLevel(gameContext);
                    }
                }
            }
        }

        private void UpdatePauseState(GameContext gameContext)
        {
            // Check if use toggled pause manually with keyboard
            if (gameContext.Root.PausePressed)
            {
                gameContext.Paused = !gameContext.Paused;
            }
            else
            {
                if (gameContext.Root.Foreground.PlayButton.Button.Pressed)
                {
                    if (gameContext.Paused)
                    {
                        gameContext.Paused = false;
                    }
                    else if (!gameContext.Paused)
                    {
                        gameContext.Paused = true;
                    }
                }
            }
        }
    }
}
