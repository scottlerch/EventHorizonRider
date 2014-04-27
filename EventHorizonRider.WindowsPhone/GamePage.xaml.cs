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

            MainGame game = XamlGame<MainGame>.Create("", this);
            game.DetailLevel = DetailLevel.CollisionDetectionHalf | DetailLevel.PixelShaderEffectsNone;
            game.TargetElapsedTime = TimeSpan.FromSeconds(1/30D);
        }
    }
}