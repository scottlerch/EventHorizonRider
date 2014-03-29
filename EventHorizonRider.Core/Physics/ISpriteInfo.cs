﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Physics
{
    internal interface ISpriteInfo
    {
        Vector2 Origin { get; }
        Vector2 Scale { get; }
        float Rotation { get; }
        Vector2 Position { get; }
        Texture2D Texture { get; }
        Color[] TextureData { get; }
    }
}