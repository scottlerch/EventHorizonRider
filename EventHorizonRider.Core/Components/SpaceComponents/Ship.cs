using EventHorizonRider.Core.Graphics;
using EventHorizonRider.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Linq;

namespace EventHorizonRider.Core.Components.SpaceComponents
{
    internal class Ship : ComponentBase
    {
        private readonly Blackhole blackhole;
        private const float DefaultRotationVelocity = MathHelper.TwoPi / 32;

        public Vector2 Position;
        public float Rotation = 0;
        public Texture2D Texture;

        private SoundEffect crashSound;
        private bool stopped = true;

        private Vector2 viewportCenter;

        public Ship(Blackhole blackhole)
        {
            this.blackhole = blackhole;
        }

        internal float Radius { get; private set; }

        protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
        {
            viewportCenter = new Vector2(graphics.Viewport.Width / 2f, graphics.Viewport.Height / 2f);

            var shipColor = Color.DarkGray.AdjustLight(0.9f);

            const int padding = 2;
            const int height = 15 + (padding * 2);
            const int width = 15 + (padding * 2);

            const float center = width / 2f;
            const float slope = center / ((float)height - (padding * 2));

            var data = new Color[width * height];

            for (var y = padding; y < height - padding; y++)
            {
                var left = (int)Math.Round(center - (slope * y));
                var right = (int)Math.Round(center + (slope * y));

                for (var x = left; x <= right; x++)
                {
                    data[x + (y * width)] = shipColor;
                }
            }

            Texture = new Texture2D(graphics, width, height, false, SurfaceFormat.Color);
            Texture.SetData(TextureProcessor.SoftenAlpha(data, width, height));

            Texture = content.Load<Texture2D>(@"Images\ship");

            crashSound = content.Load<SoundEffect>(@"Sounds\crash_sound");
        }

        protected override void DrawCore(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position,
                origin: new Vector2(Texture.Width / 2f, Texture.Height / 2f),
                rotation: Rotation,
                depth: Depth);
        }

        private bool Left(KeyboardState keyState, TouchCollection touchState)
        {
            return
                (keyState.IsKeyDown(Keys.Left) && !keyState.IsKeyDown(Keys.Right)) ||
                (touchState.Count > 0 &&
                 touchState.All(
                     t =>
                         (t.State == TouchLocationState.Pressed || t.State == TouchLocationState.Moved) &&
                         t.Position.X < viewportCenter.X));
        }

        private bool Right(KeyboardState keyState, TouchCollection touchState)
        {
            return
                (keyState.IsKeyDown(Keys.Right) && !keyState.IsKeyDown(Keys.Left)) ||
                (touchState.Count > 0 &&
                 touchState.All(
                     t =>
                         (t.State == TouchLocationState.Pressed || t.State == TouchLocationState.Moved) &&
                         t.Position.X > viewportCenter.Y));
        }

        protected override void UpdateCore(GameTime gameTime, InputState inputState)
        {
            if (stopped)
            {
                return;
            }

            Rotation += DefaultRotationVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            const float moveSpeed = 1.1f;

            if (Left(inputState.KeyState, inputState.TouchState))
            {
                Rotation -= (MathHelper.TwoPi) * (float)gameTime.ElapsedGameTime.TotalSeconds * moveSpeed;
            }

            if (Right(inputState.KeyState, inputState.TouchState))
            {
                Rotation += (MathHelper.TwoPi) * (float)gameTime.ElapsedGameTime.TotalSeconds * moveSpeed;
            }

            Rotation = MathHelper.WrapAngle(Rotation);

            const float radiusPadding = 5;

            Radius = (blackhole.Height / 2f) + (Texture.Height / 2f) + radiusPadding;

            Position.Y = blackhole.Position.Y - ((float)Math.Cos(Rotation) * Radius);
            Position.X = blackhole.Position.X + ((float)Math.Sin(Rotation) * Radius);
        }

        internal void Initialize()
        {
            Rotation = 0;

            Position.X = blackhole.Position.X;
            Position.Y = blackhole.Position.Y - (blackhole.Height / 2f) - (Texture.Height / 2f);
        }

        internal void Start()
        {
            Rotation = 0;

            Position.X = blackhole.Position.X;
            Position.Y = blackhole.Position.Y - (blackhole.Height / 2f) - (Texture.Height / 2f);

            stopped = false;
        }

        internal void Stop()
        {
            crashSound.Play();

            stopped = true;
        }
    }
}