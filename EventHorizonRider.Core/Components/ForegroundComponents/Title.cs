using EventHorizonRider.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Components.ForegroundComponents
{
    internal class Title : ComponentBase
    {
        private Texture2D texture;

        private float alpha = 1f;

        public Title()
        {
            FadeSpeed = 1f;
        }

        protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
        {
            texture = content.Load<Texture2D>(@"Images\title");
        }

        protected override void UpdateCore(GameTime gameTime, InputState inputState)
        {
            if (Visible && FadingOut)
            {
                alpha -= (float)gameTime.ElapsedGameTime.TotalSeconds*1f;
            }
        }
        public float FadeSpeed { get; set; }

        public bool FadingOut { get; set; }

        public bool Visible { get { return alpha > 0f; } }

        protected override void DrawCore(SpriteBatch spriteBatch)
        {
            if (Visible)
            {
                spriteBatch.Draw(texture, position: Vector2.Zero, color: Color.White * alpha, depth: Depth);
            }
        }
    }
}
