using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MonoGame.Framework;
using EventHorizonRider.Core;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace EventHorizonRider.WindowsPhone
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamePage : SwapChainBackgroundPanel
    {
        readonly MainGame _game;

        public GamePage(string launchArguments)
        {
            this.InitializeComponent();

            var platform = new Platform
            {
                IsMouseVisible = false,
                UseDynamicStars = false,
                PixelShaderDetail = PixelShaderDetail.Full,
                CollisionDetectionDetail = CollisionDetectionDetail.Full,
                TouchEnabled = new Windows.Devices.Input.TouchCapabilities().TouchPresent > 0,
                PauseOnLoseFocus = true,
                TargetElapsedTime = TimeSpan.FromSeconds(1 / 60D),
                IsFixedTimeStep = true,
                ParticleEffectsDetails = ParticleEffectsDetails.Full,
            };

            DeviceInfo.InitializePlatform(platform);

            _game = XamlGame<MainGame>.Create(launchArguments, Window.Current.CoreWindow, this);
        }
    }
}
