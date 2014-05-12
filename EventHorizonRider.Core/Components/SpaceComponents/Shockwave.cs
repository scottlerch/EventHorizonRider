using EventHorizonRider.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Components.SpaceComponents
{
    internal class Shockwave : ComponentBase
    {
        private readonly Blackhole blackhole;

        private Texture2D texture;
        private Vector2 textureOrigin;
        private SoundEffect sound;

        private float rotation;
        private float scale;

        private Color currentColor;
        private Color executeColor;

        public Shockwave(Blackhole newBlackhole)
        {
            blackhole = newBlackhole;
            Visible = false;
        }

        public void Execute()
        {
            Visible = true;
            scale = 0.4f;
            executeColor = currentColor;
            sound.Play();
        }

        public void SetColor(Color color)
        {
            currentColor = color;
        }

        protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
        {
            currentColor = Color.White;

            texture = content.Load<Texture2D>(@"Images\shockwave");
            sound = content.Load<SoundEffect>(@"Sounds\shockwave");

            textureOrigin = new Vector2(texture.Width/2f, texture.Height/2f);
        }

        protected override void DrawCore(SpriteBatch spriteBatch)
        {
            if (Visible)
            {
                spriteBatch.Draw(texture, blackhole.Position,
                    origin: textureOrigin,
                    rotation: rotation,
                    scale: new Vector2(scale, scale),
                    color: executeColor * 0.3f,
                    depth: Depth);
            }
        }

        protected override void UpdateCore(GameTime gameTime, InputState inputState)
        {
            rotation += blackhole.RotationalVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            scale += (float)gameTime.ElapsedGameTime.TotalSeconds*1.2f;

            if (scale > 4f)
            {
                Visible = false;
            }
        }
    }
}
