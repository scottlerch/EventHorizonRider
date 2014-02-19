using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Linq;
using EventHorizonRider.Core.Extensions;
using EventHorizonRider.Core.Utilities;
using Microsoft.Xna.Framework.Audio;

namespace EventHorizonRider.Core
{
    internal class Ship
    {
        private float DefaultRotationVelocity = MathHelper.TwoPi / 32;

        public Vector2 Position;
        public float Rotation = 0;
        public Texture2D Texture;

        private bool stopped = false;
        private GraphicsDevice graphics;

        private SoundEffect crashSound;

        public void LoadContent(ContentManager content, GraphicsDevice graphics)
        {
            this.graphics = graphics;

            var shipColor = Color.DarkGray.AdjustLight(0.9f);

            var padding = 2;
            var height = 15 + (padding * 2);
            var width = 15 + (padding * 2);

            var data = new Color[width * height];

            var center = (float)width / 2f;
            var slope = center / ((float)height - (padding * 2)); ;

            for(int y = padding; y < height - padding; y++)
            {
                var left = (int)Math.Round(center - (slope * y));
                var right = (int)Math.Round(center + (slope * y));

                for (int x = left; x <= right; x++)
                {
                    data[x + (y * width)] = shipColor;
                }
            }

            Texture = new Texture2D(graphics, width, height, false, SurfaceFormat.Color);
            Texture.SetData(TextureProcessor.SoftenAlpha(data, width, height));

            this.crashSound = content.Load<SoundEffect>("crash_sound");
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

        internal float Radius { get; private set; }

        internal void Update(KeyboardState keyState, TouchCollection touchState, GameTime gameTime, Blackhole blackhole, RingCollection rings)
        {
            if (stopped)
            {
                return;
            }

            Rotation += DefaultRotationVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

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

            const float RadiusPadding = 10;

            this.Radius = (blackhole.Height / 2) + (Texture.Height / 2) + RadiusPadding;

            Position.Y = blackhole.Position.Y - ((float)Math.Cos(Rotation) * this.Radius);
            Position.X = blackhole.Position.X + ((float)Math.Sin(Rotation) * this.Radius);

            rings.ClampToNearestGapEdge(this);
        }

        internal void Start(Blackhole blackhole)
        {
            Rotation = 0;

            Position.X = blackhole.Position.X;
            Position.Y = blackhole.Position.Y - (blackhole.Height / 2) - (Texture.Height / 2);

            stopped = false;
        }

        internal void Stop()
        {
            this.crashSound.Play();

            stopped = true;
        }
    }
}
