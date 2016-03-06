namespace EventHorizonRider.Core.Graphics
{
    /// <summary>
    /// Helper class to hold raw pixel data of a texture along with it's height and width.
    /// </summary>
    /// <typeparam name="T">Type of pixel, usually Color or uint.</typeparam>
    internal class PixelData<T>
    {
        public T[] Data { get; private set; }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public T this[int x, int y]
        {
            get { return Data[x + (y*Width)]; }
            set { Data[x + (y * Width)] = value; }
        }

        public PixelData(int width, int height)
        {
            Data = new T[width * height];
            Width = width;
            Height = height;
        }

        public PixelData(T[] data, int width, int height)
        {
            Data = data;
            Width = width;
            Height = height;
        }
    }
}
