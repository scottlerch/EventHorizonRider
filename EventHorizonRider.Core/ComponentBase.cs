using EventHorizonRider.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core
{
    internal abstract class ComponentBase
    {
        public virtual void LoadContent(ContentManager content, GraphicsDevice graphics)
        {
        }

        public virtual void Update(GameTime gameTime, InputState inputState)
        {
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
        }
    }
}