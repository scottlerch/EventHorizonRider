namespace EventHorizonRider.Core
{
    public enum PixelShaderDetail
    {
        None,
        Half,
        Full,
    }

    public enum CollisionDetectionDetail
    {
        Half,
        Full,
    }

    public class Platform
    {
        public bool PauseOnLoseFocus { get; set; }

        public bool IsMouseVisible { get; set; }

        public bool TouchEnabled { get; set; }

        public bool UseDynamicStars { get; set; }

        public bool IsPixelShaderEnabled { get { return PixelShaderDetail != PixelShaderDetail.None; } }

        public PixelShaderDetail PixelShaderDetail { get; set; }

        public CollisionDetectionDetail CollisionDetectionDetail { get; set; }

        public Platform()
        {
            IsMouseVisible = false;
            UseDynamicStars = false;
            PixelShaderDetail = PixelShaderDetail.Full;
            CollisionDetectionDetail = CollisionDetectionDetail.Full;
            TouchEnabled = true;
            PauseOnLoseFocus = true;
        }
    }
}
