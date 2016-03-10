using System;
using EventHorizonRider.Core;
using MonoGame.Framework.WindowsPhone;

namespace EventHorizonRider.WindowsPhone8
{
    public partial class GamePage
    {
        public GamePage()
        {
            InitializeComponent();

            var platform = new Platform
            {
                IsMouseVisible = false,
                UseDynamicStars = false,
                PixelShaderDetail = PixelShaderDetail.Full,
                CollisionDetectionDetail = CollisionDetectionDetail.Full,
                PauseOnLoseFocus = true,
                TouchEnabled = true,
            };

            DeviceInfo.InitializePlatform(platform);

            var game = XamlGame<MainGame>.Create("", this);
        }
    }
}