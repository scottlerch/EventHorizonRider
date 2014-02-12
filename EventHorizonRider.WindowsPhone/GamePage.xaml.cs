using EventHorizonRider.Core;
using Microsoft.Phone.Controls;
using MonoGame.Framework.WindowsPhone;

namespace EventHorizonRider.WindowsPhone
{
    public partial class GamePage : PhoneApplicationPage
    {
        private MainGame game;

        public GamePage()
        {
            InitializeComponent();

            this.game = XamlGame<MainGame>.Create("", this);
            this.game.SetResolution(1280, 768);
        }
    }
}