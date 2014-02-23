using System;
using EventHorizonRider.Core.Components;
using EventHorizonRider.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core
{
    /// <summary>
    ///     This is the main type for your game
    /// </summary>
    public class MainGame : Game
    {
        private Blackhole blackhole;
        private FpsCounter fpsCounter;
        private readonly GraphicsDeviceManager graphics;

        private InputState inputState;
        private Levels levels;

        private Music music;
        private PlayButton playButton;
        private PlayTimer playTimer;
        private PlayerData playerData;
        private RingCollection rings;
        private Ship ship;
        private readonly TimeSpan waitBetweenLevels = TimeSpan.FromSeconds(2);
        private Texture2D background;

        private Color backgroundColor = Color.LightGray;

        private int currentLevelNumber = 1;
        private TimeSpan levelEndTime = TimeSpan.Zero;
        private bool levelEnded;
        private SpriteBatch spriteBatch;
        private GameState state = GameState.Init;

        //private RenderTarget2D renderTarget;
        //private Effect grayscaleEffect;

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1136;
            graphics.PreferredBackBufferHeight = 640;
            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft;
        }

        public void SetResolution(int width, int height)
        {
            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;
        }

        /// <summary>
        ///     Allows the game to perform any initialization it needs to before starting to run.
        ///     This is where it can query for any required services and load any non-graphic
        ///     related content.  Calling base.Initialize will enumerate through any components
        ///     and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            state = GameState.Init;
            levels = new Levels();

            music = new Music();

            inputState = new InputState();

            blackhole = new Blackhole();
            ship = new Ship(blackhole);
            rings = new RingCollection(blackhole);
            fpsCounter = new FpsCounter();
            playButton = new PlayButton(() => blackhole.Scale);
            playerData = new PlayerData();
            playTimer = new PlayTimer(playerData);

            playButton.Pressed += (e, args) =>
            {
                blackhole.Pulse(1.5f, 2.5f);
                playButton.Hide();

                state = GameState.Starting;
            };

            base.Initialize();
        }

        /// <summary>
        ///     LoadContent will be called once per game and is the place to load
        ///     all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            background = Content.Load<Texture2D>("background");

            playTimer.LoadContent(Content, GraphicsDevice);
            blackhole.LoadContent(Content, GraphicsDevice);
            ship.LoadContent(Content, GraphicsDevice);
            rings.LoadContent(Content, GraphicsDevice);
            playButton.LoadContent(Content, GraphicsDevice);
            fpsCounter.LoadContent(Content, GraphicsDevice);
            music.LoadContent(Content, GraphicsDevice);

            // grayscaleEffect = Content.Load<Effect>("grayscale_effect");

            //renderTarget = new RenderTarget2D(graphics.GraphicsDevice,
            //                                  graphics.GraphicsDevice.PresentationParameters.BackBufferWidth,
            //                                  graphics.GraphicsDevice.PresentationParameters.BackBufferHeight);
        }

        /// <summary>
        ///     UnloadContent will be called once per game and is the place to unload
        ///     all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        ///     Allows the game to run logic such as updating the world,
        ///     checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            inputState.Update();

            fpsCounter.Update(gameTime, inputState);
            playButton.Update(gameTime, inputState);
            blackhole.Update(gameTime, inputState);
            playTimer.Update(gameTime, inputState);
            ship.Update(gameTime, inputState);
            rings.Update(gameTime, inputState);

            if (state == GameState.Init)
            {
                ship.Initialize();
            }
            else if (state == GameState.Starting)
            {
                currentLevelNumber = 1;

                playTimer.SetLevel(currentLevelNumber);
                playTimer.Restart();
                blackhole.Start();
                ship.Start();
                rings.Start();
                rings.SetLevel(levels.GetLevel(currentLevelNumber));
                music.Play();

                state = GameState.Running;
            }
            else if (state == GameState.Running)
            {
                backgroundColor = Color.LightGray;

                if (rings.Intersects(ship))
                {
                    state = GameState.Over;
                }

                if (!rings.HasMoreRings)
                {
                    if (!levelEnded)
                    {
                        levelEndTime = gameTime.TotalGameTime + waitBetweenLevels;
                        levelEnded = true;
                    }
                    else if (levelEndTime.Ticks < gameTime.TotalGameTime.Ticks)
                    {
                        levelEnded = false;
                        currentLevelNumber++;

                        playTimer.SetLevel(currentLevelNumber);
                        rings.SetLevel(levels.GetLevel(currentLevelNumber));
                    }
                }
            }
            else if (state == GameState.Over)
            {
                music.Stop();

                backgroundColor = Color.Red;

                blackhole.Stop();
                ship.Stop();
                rings.Stop();

                playTimer.Stop();
                playButton.Show(true);

                playerData.UpdateBestTime(playTimer.Elapsed);

                state = GameState.Paused;
            }

            base.Update(gameTime);
        }

        /// <summary>
        ///     This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.SetRenderTarget(renderTarget);

            GraphicsDevice.Clear(backgroundColor);

            if (state != GameState.Paused)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
                spriteBatch.Draw(background, Vector2.Zero);
                spriteBatch.End();
            }

            // Draw rings
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.Opaque);
            rings.Draw(spriteBatch);
            spriteBatch.End();

            // Draw blackhole and ship
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            blackhole.Draw(spriteBatch);
            ship.Draw(spriteBatch);
            spriteBatch.End();

            // Draw text
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            playTimer.Draw(spriteBatch);
            playButton.Draw(spriteBatch);
            fpsCounter.Draw(spriteBatch);
            spriteBatch.End();

            //GraphicsDevice.SetRenderTarget(null);

            //spriteBatch.Begin(0, BlendState.Opaque, null, null, null, grayscaleEffect);
            //spriteBatch.Draw((Texture2D)renderTarget, position: Vector2.Zero, color: Color.White);
            //spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}