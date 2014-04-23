using EventHorizonRider.Core.Engine;
using EventHorizonRider.Core.Engine.States;
using EventHorizonRider.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core
{
    /// <summary>
    ///     This is the main type for your game
    /// </summary>
    public class MainGame : Game
    {
        private readonly GraphicsDeviceManager graphics;

        private InputState inputState;
        private SpriteBatch spriteBatch;
        private GameContext gameContext;
        private readonly DetailLevel detailLevel;

        public MainGame() : this(DetailLevel.Full)
        {
        }

        public MainGame(DetailLevel gameDetailLevel)
        {
            detailLevel = gameDetailLevel;

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
        }

        /// <remarks>
        /// Only here for testing.  MonoGame should automatically select correct resolution.
        /// </remarks>
        public void SetResolution(int width, int height)
        {
            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;
        }

        /// <summary>
        ///     Allows the game to perform any initialization it needs to before starting to run.
        ///     This is where it can query for any required services and load any non-graphic
        ///     related content.  Calling base.Initialize will enumerate through any components
        ///     and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            DeviceInfo.Initialize(GraphicsDevice, detailLevel);

            gameContext = new GameContext(new InitializeState());
            inputState = new InputState();

            base.Initialize();
        }

        /// <summary>
        ///     LoadContent will be called once per game and is the place to load
        ///     all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            gameContext.Root.LoadContent(Content, GraphicsDevice);
        }

        /// <summary>
        ///     UnloadContent will be called once per game and is the place to unload
        ///     all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        ///     Allows the game to run logic such as updating the world,
        ///     checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            inputState.Update();
            gameContext.Update(gameTime, inputState);

            base.Update(gameTime);
        }

        /// <summary>
        ///     This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            gameContext.Root.Draw(spriteBatch, GraphicsDevice);

            base.Draw(gameTime);
        }
    }
}