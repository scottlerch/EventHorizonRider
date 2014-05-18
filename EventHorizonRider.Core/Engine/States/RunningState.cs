﻿using EventHorizonRider.Core.Components.ForegroundComponents;
using EventHorizonRider.Core.Physics;
using Microsoft.Xna.Framework;
using System;

namespace EventHorizonRider.Core.Engine.States
{
    internal class RunningState : GameStateBase
    {
        public static readonly TimeSpan WaitBetweenLevels = TimeSpan.FromSeconds(0.1);

        public TimeSpan NextLevelStartTime { get; private set; }

        public TimeSpan LevelCurrentTime { get; private set; }

        public TimeSpan TotalElapsedGameTime { get; private set; }

        public bool HasLevelEnded { get; private set; }

        public bool BestSurpassed { get; private set; }

        public override void OnBegin(GameContext gameContext, GameTime gameTime)
        {
            gameContext.Paused = false;

            BestSurpassed = false;

            gameContext.Levels.SetCurrentLevel(gameContext.PlayerData.DefaultLevelNumber);

            UpdateLevel(gameContext, gameTime, animate: false);

            gameContext.Root.Foreground.PlayButton.Show(state: PlayButtonState.Pause);
            gameContext.Root.Foreground.PlayTimer.SetLevel(gameContext.Levels.CurrentLevelNumber);
            gameContext.Root.Foreground.PlayTimer.Restart(gameContext.Levels.GetCurrentLevelStartTime());
            gameContext.Root.Foreground.PlayTimer.ShowLevelAndScore();
            gameContext.Root.Foreground.MenuButton.Hide();
            gameContext.Root.Foreground.ControlsHelp.Hide(speed: 0.2f);

            gameContext.Root.Space.Blackhole.Pulse(1.5f, 2.5f);
            gameContext.Root.Space.StopBlur();
            gameContext.Root.Space.BlackholeHalo.Visible = true;
            gameContext.Root.Space.Blackhole.SetExtraScale(0f);
            gameContext.Root.Space.Blackhole.Start();
            gameContext.Root.Space.Ship.Start();
            gameContext.Root.Space.Rings.Start();
            gameContext.Root.Space.Background.Start();
            gameContext.Root.Space.Rings.ShadowDepth = gameContext.Root.Space.Ship.Depth + 0.00001f;

            gameContext.Root.Music.Start();
        }

        public override void OnProcess(GameContext gameContext, GameTime gameTime)
        {
            UpdatePauseState(gameContext);

            if (gameContext.Paused)
            {
                gameContext.Root.Music.Pause();
                gameContext.Root.Foreground.PlayButton.Show(state:PlayButtonState.Resume);
                gameContext.Root.Space.SetBlur(blurAmount: 1.5f);
                gameContext.Root.Space.Updating = false;
                gameContext.Root.Music.Updating = false;
                gameContext.Root.Foreground.PlayTimer.Updating = false;
                return;
            }

            gameContext.Root.Music.Play();
            gameContext.Root.Foreground.PlayButton.Show(state:PlayButtonState.Pause);
            gameContext.Root.Space.SetBlur(blurAmount: 0f);
            gameContext.Root.Space.Updating = true;
            gameContext.Root.Music.Updating = true;
            gameContext.Root.Foreground.PlayTimer.Updating = true;

            var progress = 0f;

            if (gameContext.Levels.CurrentLevel.Duration.HasValue)
            {
                progress = (float)LevelCurrentTime.TotalSeconds / (float)gameContext.Levels.CurrentLevel.Duration.Value.TotalSeconds;

                // HACK: for some reason the timing is slightly off sometimes and we go past the end of level
                progress = progress > 1f ? 1f : progress;
            }

            gameContext.Root.Space.Background.StarBackgroundColor = MathUtilities.LinearInterpolate(
                gameContext.Levels.CurrentLevel.Color,
                gameContext.Levels.NextLevel.Color,
                progress);

            gameContext.Root.Foreground.PlayTimer.ProgressBar.SetProgress(progress);

            TotalElapsedGameTime += gameTime.ElapsedGameTime;

            if (!HasLevelEnded)
            {
                LevelCurrentTime += gameTime.ElapsedGameTime;
            }

            gameContext.Root.Foreground.PlayButton.Scale = gameContext.Root.Space.Blackhole.Scale.X;

            if (gameContext.Root.OverrideLevel.HasValue)
            {
                gameContext.Levels.SetCurrentLevel(gameContext.Root.OverrideLevel.Value);

                gameContext.Root.Space.Rings.Clear();
                
                UpdateLevel(gameContext, gameTime);

                gameContext.Root.OverrideLevel = null;
            }
            else
            {
                if (gameContext.Root.Space.Rings.Intersects(gameContext.Root.Space.Ship))
                {
                    gameContext.GameState = new OverState();
                }
                else if (!gameContext.Root.Space.Rings.HasMoreRings && gameContext.Root.Space.Rings.ChildrenCount == 0)
                {
                    if (!HasLevelEnded)
                    {
                        NextLevelStartTime = TotalElapsedGameTime + WaitBetweenLevels;
                        HasLevelEnded = true;
                    }
                    else if (NextLevelStartTime.Ticks < TotalElapsedGameTime.Ticks)
                    {
                        HasLevelEnded = false;
                        gameContext.Levels.IncrementCurrentLevel();

                        UpdateLevel(gameContext, gameTime);
                    }
                }
            }

            if (TotalElapsedGameTime > gameContext.PlayerData.BestTime)
            {
                if (!BestSurpassed)
                {
                    gameContext.Root.Foreground.PlayTimer.UpdateBest(TotalElapsedGameTime, isNew: true);
                    BestSurpassed = true;
                }

                gameContext.Root.Foreground.PlayTimer.UpdateBest(TotalElapsedGameTime);
            }
        }

        public override void OnEnd(GameContext gameContext, GameTime gameTime)
        {
            gameContext.Root.Music.Stop();

            gameContext.Root.Space.SetBlur(blurAmount: 5f);
            gameContext.Root.Space.BlackholeHalo.Visible = false;
            gameContext.Root.Space.Background.Gameover();
            gameContext.Root.Space.Blackhole.Stop();
            gameContext.Root.Space.Ship.Stop();
            gameContext.Root.Space.Rings.Stop();

            gameContext.Root.Foreground.PlayTimer.Stop(BestSurpassed);
            gameContext.Root.Foreground.PlayButton.Show(state: PlayButtonState.Restart);

            gameContext.IoTask = gameContext.PlayerData.UpdateBestTime(gameContext.Root.Foreground.PlayTimer.Elapsed, gameContext.Levels.CurrentLevelNumber);
        }

        private void UpdateLevel(GameContext gameContext, GameTime gameTime, bool animate = true)
        {
            LevelCurrentTime = TimeSpan.Zero;

            gameContext.Root.Foreground.PlayTimer.SetLevel(gameContext.Levels.CurrentLevelNumber, animate);
            gameContext.Root.Space.Rings.SetLevel(gameContext.Levels.CurrentLevel);
            gameContext.Root.Space.Ship.Speed = gameContext.Levels.CurrentLevel.ShipSpeed;
            gameContext.Root.Space.Shockwave.SetColor(gameContext.Levels.NextLevel.Color);
            gameContext.Root.Space.Background.RotationalVelocity = gameContext.Levels.CurrentLevel.RotationalVelocity;
            gameContext.Root.Space.Blackhole.RotationalVelocity = gameContext.Levels.CurrentLevel.RotationalVelocity;

            if (animate)
            {
                gameContext.Root.Space.Ship.Shield.Pulse();
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
