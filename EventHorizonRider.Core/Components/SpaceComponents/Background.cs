using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Components.SpaceComponents
{
    internal class Background : ComponentBase
    {
        private Texture2D background;

        private Color backgroundColor = Color.LightGray;

        protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
        {
            background = content.Load<Texture2D>(@"Images\background");
        }

        protected override void DrawCore(SpriteBatch spriteBatch)
        {
            if (backgroundColor != Color.Red)
            {
                spriteBatch.Draw(background, Vector2.Zero, depth:Depth);
            }
        }

        public Color BackgroundColor { get { return backgroundColor; } }

        public void Start()
        {
            backgroundColor = Color.LightGray;
        }

        public void Gameover()
        {
            backgroundColor = Color.Red;
        }
    }
}
