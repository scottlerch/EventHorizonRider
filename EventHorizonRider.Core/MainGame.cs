using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Storage;
using System.Diagnostics;
using System.IO;
using System.Linq;
using EventHorizonRider.Core.Extensions;
using System;
using Microsoft.Xna.Framework.Media;
using System.Linq;

namespace EventHorizonRider.Core
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MainGame : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Blackhole blackhole;
        private Ship ship;
        private RingCollection rings;
        private Levels levels;
        private PlayerData playerData;
        private PlayButton playButton;
        private PlayTimer playTimer;
        private FpsCounter fpsCounter;

        private GameState state = GameState.Init;

        private Color backgroundColor = Color.LightGray;

        private int currentLevelNumber = 1;
        private TimeSpan waitBetweenLevels = TimeSpan.FromSeconds(2);
        private TimeSpan levelEndTime = TimeSpan.Zero;
        private bool levelEnded = false;

        private Texture2D background;

        private Song musicSong;

        //private RenderTarget2D renderTarget;
        //private Effect grayscaleEffect;

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1136;
            graphics.PreferredBackBufferHeight = 640;
            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft;

            state = GameState.Init;

            rings = new RingCollection();
            ship = new Ship();
            blackhole = new Blackhole();
            levels = new Levels();
            playerData = new PlayerData();
            fpsCounter = new FpsCounter();
            playButton = new PlayButton();
            playTimer = new PlayTimer();

            playButton.Pressed += (e, args) => 
            {
                blackhole.Pulse(pullX: 1.5f, pullVelocity: 2.5f);
                playButton.Hide();

                state = GameState.Starting;
            };
        }

        public void SetResolution(int width, int height)
        {
            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            background = Content.Load<Texture2D>("background");

            playTimer.LoadContent(Content, GraphicsDevice);
            blackhole.LoadContent(Content, graphics.GraphicsDevice);
            ship.LoadContent(Content, GraphicsDevice);
            rings.LoadContent(Content, GraphicsDevice);
            playButton.LoadContent(Content, GraphicsDevice);
            fpsCounter.LoadContent(Content, graphics.GraphicsDevice);

#if !WINDOWS
            musicSong = Content.Load<Song>("techno_song");
#endif

            // grayscaleEffect = Content.Load<Effect>("grayscale_effect");

            //renderTarget = new RenderTarget2D(graphics.GraphicsDevice,
            //                                  graphics.GraphicsDevice.PresentationParameters.BackBufferWidth,
            //                                  graphics.GraphicsDevice.PresentationParameters.BackBufferHeight);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            fpsCounter.Update(gameTime);

            var touchState = TouchPanel.GetState();
            var mouseState = Mouse.GetState();
            var keyState = Keyboard.GetState();

            playButton.Update(gameTime, mouseState, touchState);
            blackhole.Update(gameTime);
            playTimer.Update(gameTime, state, playerData, currentLevelNumber);
            ship.Update(keyState, touchState, gameTime, blackhole, rings);
            rings.Update(gameTime, blackhole);
            playerData.Update(playTimer.Elapsed);

            if (state == GameState.Init)
            {
                ship.Initialize(blackhole);
            }
            else if (state == GameState.Starting)
            {
                currentLevelNumber = 1;

                playTimer.Restart();
                blackhole.Start();
                ship.Start(blackhole);
                rings.Start();
                rings.SetLevel(levels.GetLevel(currentLevelNumber));

#if !WINDOWS
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(musicSong);
#endif

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
                        rings.SetLevel(levels.GetLevel(currentLevelNumber));
                    }
                }
            }
            else if (state == GameState.Over)
            {
#if !WINDOWS
                MediaPlayer.Stop();
#endif

                backgroundColor = Color.Red;
                
                blackhole.Stop();
                ship.Stop();
                rings.Stop();

                playTimer.Stop();
                playButton.Show(isRestart: true);

                state = GameState.Paused;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
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

#if DEBUG
            fpsCounter.Draw(spriteBatch);
#endif
            spriteBatch.End();


            //GraphicsDevice.SetRenderTarget(null);

            //spriteBatch.Begin(0, BlendState.Opaque, null, null, null, grayscaleEffect);
            //spriteBatch.Draw((Texture2D)renderTarget, position: Vector2.Zero, color: Color.White);
            //spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
