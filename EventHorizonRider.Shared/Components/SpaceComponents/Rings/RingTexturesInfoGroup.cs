using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace EventHorizonRider.Core.Components.SpaceComponents.Rings;

internal class RingTexturesInfoGroup
{
    public IEnumerable<RingTexturesInfo> TextureInfos { get; private set; }

    public RingTexturesInfoGroupMode Mode { get; private set; }

    public float MaximumWidth { get; private set; }

    public RingTexturesInfoGroup(IEnumerable<RingTexturesInfo> texturesInfos, RingTexturesInfoGroupMode mode)
    {
        TextureInfos = texturesInfos;
        Mode = mode;
        MaximumWidth = TextureInfos.Max(i => 
            i.Textures.Max(t => MathHelper.Max(t.Width, t.Height)) * i.ScaleRange.High);
    }
}
