using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace EventHorizonRider.Core.Components.SpaceComponents.Rings;

internal class RingTexturesInfoGroup
{
    public IEnumerable<RingTexturesInfo> TextureInfos { get; private set; }

    public RingTexturesInfoGroupMode Mode { get; private set; }

    public float MaximumWidth { get; private set; }

#pragma warning disable IDE0350 // For compiler performance reasons, we want to use explicitly types static anonymous methods here.

    public RingTexturesInfoGroup(IEnumerable<RingTexturesInfo> texturesInfos, RingTexturesInfoGroupMode mode)
    {
        TextureInfos = texturesInfos;
        Mode = mode;
        MaximumWidth = TextureInfos.Max(static (RingTexturesInfo i) =>
            i.Textures.Max(static (Texture2D t) => MathHelper.Max(t.Width, t.Height)) * i.ScaleRange.High);
    }
}
