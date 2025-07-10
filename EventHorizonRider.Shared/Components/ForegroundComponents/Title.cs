using EventHorizonRider.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Components.ForegroundComponents
{
    internal class Title : ComponentBase
    {
        private Texture2D texture;
        private Vector2 position;
        private Vector2 origin;

        private float alpha = 1f;

        public Title()
        {
            FadeSpeed = 1f;
        }

        public float FadeSpeed { get; set; }

        public bool FadingOut { get; private set; }

        public new bool Visible { get { return alpha > 0f; } }

        public void Show()
        {
            alpha = 1f;
        }

        public void Hide()
        {
            FadingOut = true;
        }

        protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
        {
            texture = content.Load<Texture2D>(@"Images\title");

            position = new Vector2(DeviceInfo.LogicalWidth / 2f, DeviceInfo.LogicalHeight / 2f);
            origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
        }

        protected override void UpdateCore(GameTime gameTime, InputState inputState)
        {
            if (Visible && FadingOut)
            {
                alpha -= (float)gameTime.ElapsedGameTime.TotalSeconds*1f;
            }
        }

        protected override void DrawCore(SpriteBatch spriteBatch)
        {
            if (Visible)
            {
                spriteBatch.Draw(
                    texture, 
                    position,
                    origin: origin, 
                    color: Color.White * alpha, 
                    layerDepth: Depth, 
                    scale:new Vector2(1.01f),
                    sourceRectangle: null,
                    rotation: 0f,
                    effects: SpriteEffects.None);
            }
        }
    }
}
