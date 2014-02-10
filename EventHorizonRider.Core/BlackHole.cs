using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHorizonRider.Core
{
    class Blackhole
    {
        public Texture2D Texture;
        public Vector2 Position;

        internal void LoadContent(ContentManager Content, GraphicsDevice graphics)
        {
            Texture = Content.Load<Texture2D>("blackhole");

            Position = new Vector2(
                graphics.Viewport.Width / 2,
                graphics.Viewport.Height / 2);
        }

        internal void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture,
                position: Position,
                origin: new Vector2(Texture.Width / 2, Texture.Height / 2));
        }
    }
}
