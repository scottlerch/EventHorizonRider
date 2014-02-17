using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using EventHorizonRider.Core;

namespace EventHorizonRider.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
        private MainGame game;

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            game = new MainGame();
            game.Run();

            return true;
        }
    }
}

