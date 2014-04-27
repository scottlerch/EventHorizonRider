using System.Threading.Tasks;
using EventHorizonRider.Core.Components;
using EventHorizonRider.Core.Components.ForegroundComponents;
using EventHorizonRider.Core.Components.MenuComponents;
using EventHorizonRider.Core.Components.SpaceComponents;
using EventHorizonRider.Core.Input;
using Microsoft.Xna.Framework;

namespace EventHorizonRider.Core.Engine
{
    internal class GameContext
    {
        public GameStateBase GameState { get; set; }

        public Root Root { get; private set; }

        public PlayerData PlayerData { get; private set; }

        public Levels Levels { get; private set; }

        public int CurrentLevelNumber { get; set; }

        public bool Paused { get; set; }

        private Task ioTask;

        public Task IoTask
        {
            get { return ioTask; }
            set
            {
                if (ioTask != null && !ioTask.IsCompleted)
                {
                    ioTask = ioTask.ContinueWith(t => value.Wait() );
                }
                else
                {
                    ioTask = value;
                }
            }
        }

        public GameContext(GameStateBase gameState)
        {
            GameState = gameState;

            InitializeEngine();
            InitializeComponents();
        }

        private void InitializeEngine()
        {
            Levels = new Levels(new RingInfoFactory());
            PlayerData = new PlayerData();
            IoTask = PlayerData.Load();
        }

        private void InitializeComponents()
        {
            var blackhole = new Blackhole();
            var shockwave = new Shockwave(blackhole);

            Root = new Root(
                music: new Music(),
                space: new Space(
                    background: new Background(),
                    halo: new Halo(blackhole),
                    shockwave: shockwave,
                    ringCollection: new RingCollection(blackhole, shockwave),
                    ship: new Ship(blackhole),
                    blackhole: blackhole),
                menu: new Menu(
                    levelSelect: new LevelSelect(), 
                    resetButton: new ResetButton(),
                    creditsButton: new CreditsButton(),
                    credits: new Credits()),
                foreground: new Foreground(
                    playButton: new PlayButton(),
                    menuButton: new MenuButton(),
                    playTime: new PlayTimer(PlayerData),
                    title: new Title(),
                    fpsCounter: new FpsCounter()));
        }

        internal void Update(GameTime gameTime, InputState inputState)
        {
            Root.Update(gameTime, inputState);
            GameState.Handle(this, gameTime);
        }
    }
}
