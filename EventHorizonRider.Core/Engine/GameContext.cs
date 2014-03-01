using EventHorizonRider.Core.Components;
using EventHorizonRider.Core.Components.CenterComponents;
using EventHorizonRider.Core.Components.ForegroundComponents;
using EventHorizonRider.Core.Components.SpaceComponents;
using EventHorizonRider.Core.Input;
using Microsoft.Xna.Framework;

namespace EventHorizonRider.Core.Engine
{
    internal class GameContext
    {
        public Blackhole Blackhole { get; private set; }

        public Halo Halo { get; private set; }

        public FpsCounter FpsCounter { get; private set; }

        public Music Music { get; private set; }

        public PlayButton PlayButton { get; private set; }

        public PlayTimer PlayTimer { get; private set; }

        public PlayerData PlayerData { get; private set; }

        public RingCollection Rings { get; private set; }

        public Ship Ship { get; private set; }

        public Background Background { get; private set; }

        public Root Root { get; private set; }

        public Foreground Foreground { get; private set; }

        public Center Center { get; private set; }

        public Space Space { get; private set; }

        public GameStateBase GameState { get; set; }

        public Levels Levels { get; private set; }

        public int CurrentLevelNumber { get; set; }

        public GameContext(GameStateBase gameState)
        {
            GameState = gameState;

            Music = new Music();

            Blackhole = new Blackhole();
            Ship = new Ship(Blackhole);
            PlayButton = new PlayButton(Blackhole);
            Center = new Center(Blackhole, PlayButton, Ship);

            Background = new Background();
            Halo = new Halo(Blackhole);
            Rings = new RingCollection(Blackhole);
            Space = new Space(Background, Halo, Rings);

            FpsCounter = new FpsCounter();
            PlayerData = new PlayerData();
            PlayTimer = new PlayTimer(PlayerData);
            Foreground = new Foreground(PlayTimer, FpsCounter);

            Root = new Root(Music, Space, Center, Foreground);

            Levels = new Levels();
        }

        internal void Update(GameTime gameTime, InputState inputState)
        {
            Root.Update(gameTime, inputState);
            GameState.Handle(this, gameTime);
        }
    }
}
