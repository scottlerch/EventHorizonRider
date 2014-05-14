using System;
using EventHorizonRider.Core;
using MonoGame.Framework.WindowsPhone;

namespace EventHorizonRider.WindowsPhone
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
                PixelShaderDetail = PixelShaderDetail.None,
                CollisionDetectionDetail = CollisionDetectionDetail.Half,
            };

            DeviceInfo.InitializePlatform(platform);

            var game = XamlGame<MainGame>.Create("", this);
            game.TargetElapsedTime = TimeSpan.FromSeconds(1/30D);
        }
    }
}