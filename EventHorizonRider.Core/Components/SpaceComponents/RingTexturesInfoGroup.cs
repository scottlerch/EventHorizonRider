using System.Collections.Generic;

namespace EventHorizonRider.Core.Components.SpaceComponents
{
    internal enum RingTexturesInfoGroupMode
    {
        Interleave,
        Sequential,
    }

    internal class RingTexturesInfoGroup
    {
        public IEnumerable<RingTexturesInfo> TextureInfos { get; private set; }

        public RingTexturesInfoGroupMode Mode { get; private set; }

        public RingTexturesInfoGroup(IEnumerable<RingTexturesInfo> texturesInfos, RingTexturesInfoGroupMode mode)
        {
            TextureInfos = texturesInfos;
            Mode = mode;
        }
    }
}
