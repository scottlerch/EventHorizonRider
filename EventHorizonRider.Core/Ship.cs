using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Linq;

namespace EventHorizonRider.Core
{
    internal class Ship
    {
        public Vector2 Position;
        public float Rotation = 0;
        public Texture2D Texture;

        private bool stopped = false;
        private GraphicsDevice graphics;

        public void LoadContent(ContentManager content, GraphicsDevice graphics)
        {
            // Texture = content.Load<Texture2D>("ship");
            this.graphics = graphics;

            var shipColor = Color.DarkGray.AdjustLight(0.9f).PackedValue;

            var height = 15;
            var width = 15;

            uint[] data = new uint[width * height];

            var center = (float)width / 2f;
            var slope = center / (float)height;

            for(int y = 0; y < height; y++)
            {
                var left = (int)Math.Round(center - (slope * y));
                var right = (int)Math.Round(center + (slope * y));

                for (int x = left; x <= right; x++)
                {
                    data[x + (y * width)] = shipColor;
                }
            }

            Texture = new Texture2D(graphics, width, height, false, SurfaceFormat.Color);
            Texture.SetData(data);
        }

        public void Update(GameTime gameTime)
        {

        }

        internal void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture,
                position: Position,
                origin: new Vector2(Texture.Width / 2, Texture.Height / 2),
                rotation: Rotation);
        }

        private bool Left(KeyboardState keyState, TouchCollection touchState)
        {
            return
                (keyState.IsKeyDown(Keys.Left) && !keyState.IsKeyDown(Keys.Right)) ||
                (touchState.Count > 0 && touchState.All(t => (t.State == TouchLocationState.Pressed || t.State == TouchLocationState.Moved) && t.Position.X < graphics.Viewport.Width / 2));
        }

        private bool Right(KeyboardState keyState, TouchCollection touchState)
        {
            return 
                (keyState.IsKeyDown(Keys.Right) && !keyState.IsKeyDown(Keys.Left)) ||
                (touchState.Count > 0 && touchState.All(t => (t.State == TouchLocationState.Pressed || t.State == TouchLocationState.Moved) && t.Position.X > graphics.Viewport.Width / 2));
        }

        internal void Update(KeyboardState keyState, TouchCollection touchState, GameTime gameTime, Blackhole blackhole)
        {
            if (stopped)
            {
                return;
            }

            float moveSpeed = 1.1f;

            if (Left(keyState, touchState))
            {
                Rotation -= (MathHelper.TwoPi) * (float)gameTime.ElapsedGameTime.TotalSeconds * moveSpeed;
            }

            if (Right(keyState, touchState))
            {
                Rotation += (MathHelper.TwoPi) * (float)gameTime.ElapsedGameTime.TotalSeconds * moveSpeed;
            }

            Rotation = MathHelper.WrapAngle(Rotation);

            var radius = (blackhole.Texture.Width / 2) + (Texture.Height / 2);
            Position.Y = blackhole.Position.Y - ((float)Math.Cos(Rotation) * radius);
            Position.X = blackhole.Position.X + ((float)Math.Sin(Rotation) * radius);
        }

        internal void Initialize(Blackhole blackhole)
        {
            Rotation = 0;

            Position.X = blackhole.Position.X;
            Position.Y = blackhole.Position.Y - (blackhole.Texture.Height / 2) - (Texture.Height / 2);

            stopped = false;
        }

        internal void Stop()
        {
            stopped = true;
        }
    }
}
