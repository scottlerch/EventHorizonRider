using EventHorizonRider.Core;
using System.Windows.Forms;

namespace EventHorizonRider.Windows
{
    public partial class DevelopmentToolsForm : Form
    {
        private MainGame mainGame;

        public DevelopmentToolsForm(MainGame game)
        {
            mainGame = game;
            mainGame.Initialized += OnGameInitialized;
            InitializeComponent();
        }

        private void OnGameInitialized(object sender, System.EventArgs e)
        {
            propertyGrid.SelectedObject = mainGame.GameContext;
        }
    }
}
