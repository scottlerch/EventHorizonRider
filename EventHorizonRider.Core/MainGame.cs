using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Storage;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace EventHorizonRider.Core
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MainGame : Game
    {
        private enum GameState
        {
            Init,
            Running,
            Paused,
            Over,
        }

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Blackhole blackhole;
        private Ship ship;
        private RingCollection rings;
        private Levels levels;
        private PlayerData playerData;

        private SpriteFont spriteFont;

        private Stopwatch gameTimeElapsed;

        private GameState state;

        private Color backgroundColor = Color.LightGray;

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

            spriteFont = Content.Load<SpriteFont>("font2");

            blackhole.LoadContent(Content, graphics.GraphicsDevice);
            ship.LoadContent(Content, GraphicsDevice);
            rings.LoadContent(GraphicsDevice);
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
            var touchState = TouchPanel.GetState();
            var mouseState = Mouse.GetState();
            var keyState = Keyboard.GetState();

            if (touchState.Any(t => t.State == TouchLocationState.Pressed && t.Position.X > (graphics.GraphicsDevice.Viewport.Width - 200) && t.Position.Y < 50) ||
                (mouseState.X > (graphics.GraphicsDevice.Viewport.Width - 200) && mouseState.Y < 50))
            {
                state = GameState.Init;
            }

            if (state == GameState.Init)
            {
                gameTimeElapsed = Stopwatch.StartNew();

                ship.Initialize(blackhole);
                rings.Initialize();
                rings.SetLevel(levels.GetLevelOne());

                state = GameState.Running;
            }
            else if (state == GameState.Running)
            {
                backgroundColor = Color.LightGray;

                ship.Update(keyState, touchState, gameTime, blackhole, rings);
                rings.Update(gameTime);

                if (rings.Intersects(ship))
                {
                    state = GameState.Over;
                }
            }
            else if (state == GameState.Over)
            {
                backgroundColor = Color.Red;

                ship.Stop();
                rings.Stop();
                gameTimeElapsed.Stop();

                playerData.Update(gameTimeElapsed.Elapsed);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(backgroundColor);

            // Draw rings
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            rings.Draw(spriteBatch);
            spriteBatch.End();

            // Draw blackhole and ship
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            blackhole.Draw(spriteBatch);
            ship.Draw(spriteBatch);
            spriteBatch.End();

            // Draw text
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            var scoreColor = Color.White;

            if (gameTimeElapsed.Elapsed >= playerData.Highscore)
            {
                scoreColor = Color.Yellow;
            }
            else
            {
                var percentComplete = 1f - (float)(gameTimeElapsed.Elapsed.TotalSeconds / playerData.Highscore.TotalSeconds);

                scoreColor = Color.White.SetColors(percentComplete, 1f, percentComplete);
            }

            spriteBatch.DrawString(spriteFont, gameTimeElapsed.Elapsed.ToString("hh\\:mm\\:ss\\.ff"), new Vector2(10, 10), scoreColor, 0, Vector2.Zero, 1, SpriteEffects.None, 0.1f);
            spriteBatch.DrawString(spriteFont, gameTimeElapsed.Elapsed.ToString("hh\\:mm\\:ss\\.ff"), new Vector2(12, 12), Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 0.2f);

            spriteBatch.DrawString(spriteFont, "Highscore: " + playerData.Highscore.ToString("hh\\:mm\\:ss\\.ff"), new Vector2(10, 10 + 20), Color.LightGray.AdjustLight(0.9f), 0, Vector2.Zero, 1, SpriteEffects.None, 0.1f);
            spriteBatch.DrawString(spriteFont, "Highscore: " + playerData.Highscore.ToString("hh\\:mm\\:ss\\.ff"), new Vector2(12, 12 + 20), Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 0.2f);

            var rightEdge = graphics.GraphicsDevice.Viewport.Width - 110;
            spriteBatch.DrawString(spriteFont, "RESTART", new Vector2(rightEdge, 10), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.1f);
            spriteBatch.DrawString(spriteFont, "RESTART", new Vector2(rightEdge + 2, 12), Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 0.2f);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
