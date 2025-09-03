using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

namespace EventHorizonRider.Core.Graphics;

/// <summary>
/// Helper methods to process texture data like scaling, cropping, anti-aliasing, etc.
/// </summary>
internal class TextureProcessor
{
    public static PixelData<byte> GetScaledData(PixelData<byte> data, float scale)
    {
        var scaledData = new PixelData<byte>((int)Math.Round(data.Width * scale), (int)Math.Round(data.Height * scale));

        var xRatio = data.Width / (float)scaledData.Width;
        var yRatio = data.Height / (float)scaledData.Height;

        for (var y = 0; y < scaledData.Height; y++)
        {
            for (var x = 0; x < scaledData.Width; x++)
            {
                scaledData[x, y] = data[(int)(x * xRatio), (int)(y * yRatio)];
            }
        }

        return scaledData;
    }

    public static PixelData<byte> GetCroppedData(PixelData<byte> data, Rectangle bounds)
    {
        var croppedWidth = bounds.Width;
        var croppedHeight = bounds.Height;
        var croppedData = new PixelData<byte>(croppedWidth, croppedHeight);

        for (var x = 0; x < croppedWidth; x++)
        {
            for (var y = 0; y < croppedHeight; y++)
            {
                croppedData[x, y] = data[x + bounds.X, y + bounds.Y];
            }
        }

        return croppedData;
    }

    public static PixelData<byte> GetAlphaData(Texture2D texture)
    {
        var data = new Color[texture.Width * texture.Height];
        texture.GetData(data);
        return new PixelData<byte>([.. data.Select(c => c.A)], texture.Width, texture.Height);
    }

    public static Color[] SoftenAlpha(Color[] data, int width, int height)
    {
        var antiAliasedData = new Color[width * height];

        for (var x = 1; x < width - 1; x++)
        {
            for (var y = 1; y < height - 1; y++)
            {
                var color = data[x + (y * width)];
                antiAliasedData[x + (y * width)] = new Color(color.R, color.G, color.B,
                    data[x + (y * width)].A / 2 +
                    data[(x + 1) + (y * width)].A / 8 +
                    data[(x - 1) + (y * width)].A / 8 +
                    data[x + ((y - 1) * width)].A / 8 +
                    data[x + ((y + 1) * width)].A / 8 +
                    data[(x + 1) + ((y + 1) * width)].A / 16 +
                    data[(x - 1) + ((y - 1) * width)].A / 16 +
                    data[(x - 1) + ((y + 1) * width)].A / 16 +
                    data[(x + 1) + ((y - 1) * width)].A / 16);
            }
        }

        return antiAliasedData;
    }
}
