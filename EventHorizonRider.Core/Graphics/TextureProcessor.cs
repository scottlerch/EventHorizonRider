using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace EventHorizonRider.Core.Graphics
{
    internal class TextureProcessor
    {
        public static byte[] GetAlphaData(Texture2D texture)
        {
            var data = new Color[texture.Width * texture.Height];
            texture.GetData(data);
            return data.Select(c => c.A).ToArray();
        }

        public static Color[] SoftenAlpha(Color[] data, int width, int height)
        {
            var antiAliasedData = new Color[width*height];

            for (var x = 1; x < width - 1; x++)
            {
                for (var y = 1; y < height - 1; y++)
                {
                    var color = data[x + (y*width)];
                    antiAliasedData[x + (y*width)] = new Color(color.R, color.G, color.B,
                        data[x + (y*width)].A/2 +
                        data[(x + 1) + (y*width)].A/8 +
                        data[(x - 1) + (y*width)].A/8 +
                        data[x + ((y - 1)*width)].A/8 +
                        data[x + ((y + 1)*width)].A/8 +
                        data[(x + 1) + ((y + 1)*width)].A/16 +
                        data[(x - 1) + ((y - 1)*width)].A/16 +
                        data[(x - 1) + ((y + 1)*width)].A/16 +
                        data[(x + 1) + ((y - 1)*width)].A/16);
                }
            }

            return antiAliasedData;
        }
    }
}