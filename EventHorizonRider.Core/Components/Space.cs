using EventHorizonRider.Core.Components.SpaceComponents;
using EventHorizonRider.Core.Graphics;
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
        private readonly Background background;

        public bool Blur { get; set; }

        public Space(Background background, Halo halo, Shockwave shockwave, RingCollection ringCollection, Blackhole blackhole, Ship ship) 
            : base(background, halo, shockwave, ringCollection, blackhole, ship)
        {
            this.background = background;
        }

        protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
        {
            renderTarget1 = new RenderTarget2D(graphics,
                                              graphics.PresentationParameters.BackBufferWidth,
                                              graphics.PresentationParameters.BackBufferHeight);

            renderTarget2 = new RenderTarget2D(graphics,
                                  graphics.PresentationParameters.BackBufferWidth / 2,
                                  graphics.PresentationParameters.BackBufferHeight / 2);

            blur.LoadContent(content);
        }

        protected override void OnBeforeDraw(SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            if (Blur)
            {
                graphics.SetRenderTarget(renderTarget1);
            }

            graphics.Clear(background.BackgroundColor);

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
        }

        protected override void OnAfterDraw(SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            spriteBatch.End();

            if (Blur)
            {
                graphics.SetRenderTarget(renderTarget2);

                blur.SetBlurEffectParameters(1f / renderTarget2.Width, 0f);
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
