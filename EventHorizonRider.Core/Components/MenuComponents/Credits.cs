﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Components.MenuComponents
{
    internal class Credits : ComponentBase
    {
        private Texture2D texture;
        private Vector2 center;

        protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
        {
            Visible = false;

            texture = content.Load<Texture2D>(@"Images\credits");

            center = new Vector2(
                DeviceInfo.LogicalWidth / 2f,
                DeviceInfo.LogicalHeight / 2f);
        }

        protected override void DrawCore(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                texture,
                origin: new Vector2(texture.Width/2f, texture.Height/2f),
                position: center,
                scale: Vector2.One,
                color: Color.White,
                depth: Depth);
        }
    }
}