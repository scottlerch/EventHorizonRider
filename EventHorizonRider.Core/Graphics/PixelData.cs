namespace EventHorizonRider.Core.Graphics
{
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
