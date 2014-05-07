using EventHorizonRider.Core.Components.ForegroundComponents;
using EventHorizonRider.Core.Physics;
using Microsoft.Xna.Framework;
using System;

namespace EventHorizonRider.Core.Engine.States
{
    internal class RunningState : GameStateBase
    {
        public static readonly TimeSpan WaitBetweenLevels = TimeSpan.FromSeconds(0.1);

        public Level CurrentLevel { get; private set; }

        public Level NextLevel { get; private set; }

        public TimeSpan NextLevelStartTime { get; private set; }

        public TimeSpan LevelCurrentTime { get; private set; }

        public TimeSpan TotalElapsedGameTime { get; private set; }

        public bool HasLevelEnded { get; private set; }

        public bool BestSurpassed { get; private set; }

        public override void OnBegin(GameContext gameContext, GameTime gameTime)
        {
            BestSurpassed = false;

            gameContext.CurrentLevelNumber = gameContext.PlayerData.DefaultLevelNumber;
            CurrentLevel = gameContext.LevelCollection.GetLevel(gameContext.CurrentLevelNumber);
            NextLevel = gameContext.LevelCollection.GetLevel(gameContext.CurrentLevelNumber + 1);

            gameContext.Root.Foreground.PlayButton.Show(state: PlayButtonState.Pause);
            gameContext.Root.Foreground.PlayTimer.SetLevel(gameContext.CurrentLevelNumber);
            gameContext.Root.Foreground.PlayTimer.Restart(gameContext.LevelCollection.GetLevelStartTime(gameContext.CurrentLevelNumber));
            gameContext.Root.Foreground.PlayTimer.ShowLevelAndScore();
            gameContext.Root.Foreground.MenuButton.Hide();
            gameContext.Root.Foreground.ControlsHelp.Hide(speed: 0.2f);

            gameContext.Root.Space.Blackhole.Pulse(1.5f, 2.5f);
            gameContext.Root.Space.StopBlur();
            gameContext.Root.Space.Halo.Visible = true;
            gameContext.Root.Space.Blackhole.SetExtraScale(0f);
            gameContext.Root.Space.Blackhole.Start();
            gameContext.Root.Space.Ship.Start();
            gameContext.Root.Space.Rings.Start();
            gameContext.Root.Space.Background.Start();
            gameContext.Root.Space.Rings.SetLevel(CurrentLevel);
            gameContext.Root.Space.Ship.Speed = CurrentLevel.ShipSpeed;
            gameContext.Root.Space.Shockwave.SetColor(NextLevel.Color);

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

            if (CurrentLevel.Duration.HasValue)
            {
                var progress = LevelCurrentTime.TotalSeconds/CurrentLevel.Duration.Value.TotalSeconds;

                gameContext.Root.Space.Background.StarBackgroundColor = MathUtilities.LinearInterpolate(
                    CurrentLevel.Color,
                    NextLevel.Color,
                    progress);

                gameContext.Root.Foreground.PlayTimer.SetProgress((float)progress);
            }

            TotalElapsedGameTime += gameTime.ElapsedGameTime;

            if (!HasLevelEnded)
            {
                LevelCurrentTime += gameTime.ElapsedGameTime;
            }

            gameContext.Root.Foreground.PlayButton.Scale = gameContext.Root.Space.Blackhole.Scale.X;

            if (gameContext.Root.OverrideLevel.HasValue)
            {
                gameContext.CurrentLevelNumber = gameContext.Root.OverrideLevel.Value;

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
                        gameContext.CurrentLevelNumber++;

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
            gameContext.Root.Space.Halo.Visible = false;
            gameContext.Root.Space.Background.Gameover();
            gameContext.Root.Space.Blackhole.Stop();
            gameContext.Root.Space.Ship.Stop();
            gameContext.Root.Space.Rings.Stop();

            gameContext.Root.Foreground.PlayTimer.Stop(BestSurpassed);
            gameContext.Root.Foreground.PlayButton.Show(state: PlayButtonState.Restart);

            gameContext.IoTask = gameContext.PlayerData.UpdateBestTime(gameContext.Root.Foreground.PlayTimer.Elapsed, gameContext.CurrentLevelNumber);
        }

        private void UpdateLevel(GameContext gameContext, GameTime gameTime)
        {
            LevelCurrentTime = TimeSpan.Zero;

            CurrentLevel = gameContext.LevelCollection.GetLevel(gameContext.CurrentLevelNumber);
            NextLevel = gameContext.LevelCollection.GetLevel(gameContext.CurrentLevelNumber + 1);

            gameContext.Root.Foreground.PlayTimer.SetLevel(gameContext.CurrentLevelNumber, animate: true);
            gameContext.Root.Space.Rings.SetLevel(CurrentLevel);
            gameContext.Root.Space.Ship.Speed = CurrentLevel.ShipSpeed;
            gameContext.Root.Space.Shockwave.SetColor(NextLevel.Color);
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
