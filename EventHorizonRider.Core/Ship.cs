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
    class Ship
    {
        public Vector2 Position;
        public float Rotation = 0;
        public Texture2D Texture;

        public void LoadContent(ContentManager content)
        {
            Texture = content.Load<Texture2D>("ship");
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
    }
}
