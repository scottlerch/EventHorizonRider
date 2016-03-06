using EventHorizonRider.Core.Components;
using EventHorizonRider.Core.Components.ForegroundComponents;
using EventHorizonRider.Core.Components.MenuComponents;
using EventHorizonRider.Core.Components.SpaceComponents;
using EventHorizonRider.Core.Components.SpaceComponents.Rings;
using EventHorizonRider.Core.Input;
using Microsoft.Xna.Framework;
using System.Threading.Tasks;

namespace EventHorizonRider.Core.Engine
{
    internal class GameContext
    {
        /// <summary>
        /// This is the current state of the game.  The logic for each state is contained
        /// in subclasses of GameStateBase in the States namespace.
        /// </summary>
        public GameStateBase GameState { get; set; }

        /// <summary>
        /// This is the root components of all (mostly) visible game components.
        /// </summary>
        public Root Root { get; private set; }

        /// <summary>
        /// This is the persisted user data like high score.
        /// </summary>
        public PlayerData PlayerData { get; private set; }

        /// <summary>
        /// This contains all the information about the levels (how to generate), current, next, timings, etc.
        /// </summary>
        public LevelCollection Levels { get; private set; }

        /// <summary>
        /// Indicates if the game is paused or not.  
        /// This really only applies to when the GameState is set to RunningState.
        /// </summary>
        public bool Paused { get; set; }

        private Task ioTask;

        /// <summary>
        /// This is the task where all IO operations are performed in a non-blocking way.
        /// </summary>
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
            Levels = new LevelCollection(new RingInfoFactory());
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
                    background: new Background(new StarFactory()),
                    blackholeHalo: new BlackholeHalo(blackhole),
                    shockwave: shockwave,
                    ringCollection: new RingCollection(blackhole, shockwave, new RingFactory()),
                    ship: new Ship(blackhole),
                    blackhole: blackhole),
                menu: new Menu(
                    levelSelect: new LevelSelect(Levels), 
                    resetButton: new ResetButton(),
                    creditsButton: new CreditsButton(),
                    credits: new Credits()),
                foreground: new Foreground(
                    playButton: new PlayButton(),
                    menuButton: new MenuButton(),
                    playTime: new PlayTimer(Levels),
                    title: new Title(),
                    controlsHelp: new ControlsHelp(), 
                    fpsCounter: new FpsCounter()));
        }

        internal void Update(GameTime gameTime, InputState inputState)
        {
            Root.Update(gameTime, inputState);
            GameState.Handle(this, gameTime);
        }
    }
}
