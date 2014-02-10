using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace EventHorizonRider.Core
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MainGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Blackhole blackhole;
        Ship ship;
        RingCollection rings;

        SpriteFont spriteFont;

        Stopwatch gameTimeElapsed;

        enum GameState
        {
            Init,
            Running,
            Paused,
            Over,
        }

        GameState state;

        Color backgroundColor = Color.LightGray;

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1136;
            graphics.PreferredBackBufferHeight = 640;

            state = GameState.Init;

            rings = new RingCollection();
            ship = new Ship();
            blackhole = new Blackhole();
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
            ship.LoadContent(Content);
            rings.LoadContent(GraphicsDevice);
        }

        void InitShip()
        {
            ship.Position.X = blackhole.Position.X;
            ship.Position.Y = blackhole.Position.Y - (blackhole.Texture.Height / 2) - (ship.Texture.Height / 2);
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

            if (touchState.Any(t => t.State == TouchLocationState.Pressed && t.Position.X > (graphics.GraphicsDevice.Viewport.Width - 200) && t.Position.Y < 30) ||
                (mouseState.X > (graphics.GraphicsDevice.Viewport.Width - 200) && mouseState.Y < 30))
            {
                state = GameState.Init;
            }

            if (state == GameState.Init)
            {
                gameTimeElapsed = Stopwatch.StartNew();
                InitShip();
                rings.Clear();
                state = GameState.Running;
            }
            else if (state == GameState.Running)
            {
                gameTimeElapsed.Start();
                backgroundColor = Color.LightGray;

                KeyboardState keyState = Keyboard.GetState();

                float moveSpeed = 1.1f;
                if (keyState.IsKeyDown(Keys.Left) && !keyState.IsKeyDown(Keys.Right))
                {
                    ship.Rotation -= (MathHelper.Pi * 2) * (float)gameTime.ElapsedGameTime.TotalSeconds * moveSpeed;
                }

                if (keyState.IsKeyDown(Keys.Right) && !keyState.IsKeyDown(Keys.Left))
                {
                    ship.Rotation += (MathHelper.Pi * 2) * (float)gameTime.ElapsedGameTime.TotalSeconds * moveSpeed;
                }

                ship.Rotation = MathHelper.WrapAngle(ship.Rotation);

                var radius = (blackhole.Texture.Width / 2) + (ship.Texture.Height / 2);
                ship.Position.Y = blackhole.Position.Y - ((float)Math.Cos(ship.Rotation) * radius);
                ship.Position.X = blackhole.Position.X + ((float)Math.Sin(ship.Rotation) * radius);

                foreach (var ring in rings.GetRings())
                {
                    if (!(
                            (ship.Rotation < ring.GapAngle + 0.5f) &&
                            (ship.Rotation > ring.GapAngle - 0.5f)
                          ) &&
                        (ring.Radius < 50f && ring.Radius > 25f))
                    {
                        state = GameState.Over;
                    }
                }

                rings.Update(gameTime);
            }
            else if (state == GameState.Over)
            {
                backgroundColor = Color.Red;
                rings.Clear();
                gameTimeElapsed.Stop();
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

            spriteBatch.DrawString(spriteFont, gameTimeElapsed.Elapsed.ToString("hh\\:mm\\:ss\\.ff"), new Vector2(10, 10), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.1f);
            spriteBatch.DrawString(spriteFont, gameTimeElapsed.Elapsed.ToString("hh\\:mm\\:ss\\.ff"), new Vector2(12, 12), Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 0.2f);

            var rightEdge = graphics.GraphicsDevice.Viewport.Width - 110;
            spriteBatch.DrawString(spriteFont, "RESTART", new Vector2(rightEdge, 10), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.1f);
            spriteBatch.DrawString(spriteFont, "RESTART", new Vector2(rightEdge + 2, 12), Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 0.2f);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
