using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace EventHorizonRider.Core
{
    class Ring
    {
        public Texture2D Texture;
        public Vector2 Position;
        public Vector2 Origin;
        public float Rotation;
        public Vector2 Scale;
    }

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MainGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D blackholeTexture;
        Texture2D shipTexture;

        Vector2 blackholePosition;
        Vector2 shipPosition;
        float shipRotation = 0;

        List<Ring> rings = new List<Ring>();

        SpriteFont spriteFont;

        Stopwatch gameTimeElapsed;

        Random rand = new Random();

        Texture2D ringTexture;

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
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            spriteFont = Content.Load<SpriteFont>("font2");

            blackholeTexture = Content.Load<Texture2D>("blackhole");
            shipTexture = Content.Load<Texture2D>("ship");

            blackholePosition = new Vector2(
                graphics.GraphicsDevice.Viewport.Width / 2,
                graphics.GraphicsDevice.Viewport.Height / 2);

            ringTexture = Content.Load<Texture2D>("ring_1gap");
        }

        void InitShip()
        {
            shipPosition.X = blackholePosition.X;
            shipPosition.Y = blackholePosition.Y - (blackholeTexture.Height / 2) - (shipTexture.Height / 2);
        }

        void AddRing()
        {
            rings.Add(new Ring
            {
                Texture = ringTexture,
                Position = blackholePosition,
                Origin = new Vector2(ringTexture.Width / 2, ringTexture.Height / 2),
                Rotation = MathHelper.WrapAngle((float)rand.NextDouble() * MathHelper.Pi * 2f),
                Scale = new Vector2(1, 1),
            });
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
            var mouseState = Mouse.GetState();
            if (mouseState.X > (graphics.GraphicsDevice.Viewport.Width - 200) && mouseState.Y < 30)
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
                    shipRotation -= (MathHelper.Pi * 2) * (float)gameTime.ElapsedGameTime.TotalSeconds * moveSpeed;
                }

                if (keyState.IsKeyDown(Keys.Right) && !keyState.IsKeyDown(Keys.Left))
                {
                    shipRotation += (MathHelper.Pi * 2) * (float)gameTime.ElapsedGameTime.TotalSeconds * moveSpeed;
                }

                shipRotation = MathHelper.WrapAngle(shipRotation);

                var radius = (blackholeTexture.Width / 2) + (shipTexture.Height / 2);
                shipPosition.Y = blackholePosition.Y - ((float)Math.Cos(shipRotation) * radius);
                shipPosition.X = blackholePosition.X + ((float)Math.Sin(shipRotation) * radius);

                foreach (var ring in rings.ToList())
                {
                    ring.Scale.X -= (float)gameTime.ElapsedGameTime.TotalSeconds * 0.5f;
                    ring.Scale.Y -= (float)gameTime.ElapsedGameTime.TotalSeconds * 0.5f;

                    if (!(
                            (shipRotation < ring.Rotation + 0.5f) &&
                            (shipRotation > ring.Rotation - 0.5f)
                          ) &&
                        (ring.Scale.X < 0.05f && ring.Scale.X > 0.03f))
                    {
                        state = GameState.Over;
                    }

                    if (ring.Scale.X <= 0)
                    {
                        rings.Remove(ring);
                    }
                }

                if (DateTime.UtcNow - lastRingAdd > TimeSpan.FromSeconds(0.9))
                {
                    lastRingAdd = DateTime.UtcNow;
                    AddRing();
                }
            }
            else if (state == GameState.Over)
            {
                backgroundColor = Color.Red;
                rings.Clear();
                gameTimeElapsed.Stop();
            }

            base.Update(gameTime);
        }

        DateTime lastRingAdd = DateTime.UtcNow;

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(backgroundColor);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            foreach (var ring in rings)
            {
                spriteBatch.Draw(ring.Texture,
                    position: ring.Position,
                    rotation: ring.Rotation,
                    origin: ring.Origin,
                    scale: ring.Scale);
            }

            spriteBatch.Draw(blackholeTexture, 
                position: blackholePosition, 
                origin: new Vector2(blackholeTexture.Width / 2, blackholeTexture.Height / 2));

            spriteBatch.Draw(shipTexture,
                position: shipPosition,
                origin: new Vector2(shipTexture.Width / 2, shipTexture.Height / 2),
                rotation: shipRotation);

            spriteBatch.End();

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
