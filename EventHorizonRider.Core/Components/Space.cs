using System;
using EventHorizonRider.Core.Components.SpaceComponents;
using EventHorizonRider.Core.Graphics;
using EventHorizonRider.Core.Input;
using EventHorizonRider.Core.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Components
{
    internal class Space : ComponentBase
    {
        private RenderTarget2D renderTarget1;
        private RenderTarget2D renderTarget2;
        private readonly GaussianBlur blur = new GaussianBlur();

        private Motion blurAmountMotion;

        public Blackhole Blackhole { get; private set; }

        public Halo Halo { get; private set; }

        public RingCollection Rings { get; private set; }

        public Ship Ship { get; private set; }

        public Background Background { get; private set; }

        public Shockwave Shockwave { get; private set; }

        public void SetBlur(float blurAmount)
        {
            blurAmountMotion.Set(blurAmount);
        }

        public void StartBlur(float blurAmount, float speed)
        {
            if (!DeviceInfo.DetailLevel.HasFlag(DetailLevel.PixelShaderEffectsNone))
            {
                blurAmountMotion.UpdateTarget(blurAmount, speed);
            }
        }

        public void StopBlur()
        {
            if (!DeviceInfo.DetailLevel.HasFlag(DetailLevel.PixelShaderEffectsNone))
            {
                blurAmountMotion.UpdateTarget(0);
            }
        }

        public Space(Background background, Halo halo, Shockwave shockwave, RingCollection ringCollection, Ship ship, Blackhole blackhole) 
            : base(background, halo, shockwave, ship,ringCollection, blackhole)
        {
            blurAmountMotion = new Motion();
            blurAmountMotion.Initialize(0, 0, 30);

            Background = background;
            Halo = halo;
            Shockwave = shockwave;
            Rings = ringCollection;
            Ship = ship;
            Blackhole = blackhole;
        }

        protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
        {
            if (!DeviceInfo.DetailLevel.HasFlag(DetailLevel.PixelShaderEffectsNone))
            {
                renderTarget1 = new RenderTarget2D(graphics,
                    graphics.PresentationParameters.BackBufferWidth,
                    graphics.PresentationParameters.BackBufferHeight);

                var scaleBackBuffer = 2f;

                if (DeviceInfo.DetailLevel == DetailLevel.PixelShaderEffectsHalf)
                {
                    scaleBackBuffer = 4f;
                }

                renderTarget2 = new RenderTarget2D(graphics,
                    (int)
                        Math.Round((graphics.PresentationParameters.BackBufferWidth/scaleBackBuffer)*
                                   DeviceInfo.InputScale),
                    (int)
                        Math.Round((graphics.PresentationParameters.BackBufferHeight/scaleBackBuffer)*
                                   DeviceInfo.InputScale));

                blur.LoadContent(content);
            }
        }

        protected override void UpdateCore(GameTime gameTime, InputState inputState)
        {
            blurAmountMotion.Update(gameTime);
            blur.BlueAmount = blurAmountMotion.Value*DeviceInfo.OutputScale;
        }

        protected override void OnBeforeDraw(SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            if (!DeviceInfo.DetailLevel.HasFlag(DetailLevel.PixelShaderEffectsNone))
            {
                if (blurAmountMotion.Value > 0)
                {
                    graphics.SetRenderTarget(renderTarget1);
                }
            }

            graphics.Clear(Background.BackgroundColor);

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, DeviceInfo.OutputScaleMatrix);
        }

        protected override void OnAfterDraw(SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            spriteBatch.End();

            if (!DeviceInfo.DetailLevel.HasFlag(DetailLevel.PixelShaderEffectsNone))
            {
                if (blurAmountMotion.Value > 0)
                {
                    graphics.SetRenderTarget(renderTarget2);

                    blur.SetBlurEffectParameters(1f/renderTarget2.Width, 0f);
                    spriteBatch.Begin(0, BlendState.Opaque, null, null, null, blur.Effect);
                    spriteBatch.Draw(renderTarget1, renderTarget2.Bounds, Color.White);
                    spriteBatch.End();

                    graphics.SetRenderTarget(null);

                    blur.SetBlurEffectParameters(0f, 1f/renderTarget1.Height);
                    spriteBatch.Begin(0, BlendState.Opaque, null, null, null, blur.Effect);
                    spriteBatch.Draw(renderTarget2, renderTarget1.Bounds, Color.White);
                    spriteBatch.End();
                }
            }
        }
    }
}
