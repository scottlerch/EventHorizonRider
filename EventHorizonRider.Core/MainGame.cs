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

        //private RenderTarget2D renderTarget;
        //private Effect grayscaleEffect;

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1136;
            graphics.PreferredBackBufferHeight = 640;
            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
        }

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

            // grayscaleEffect = Content.Load<Effect>("grayscale_effect");

            //renderTarget = new RenderTarget2D(graphics.GraphicsDevice,
            //                                  graphics.GraphicsDevice.PresentationParameters.BackBufferWidth,
            //                                  graphics.GraphicsDevice.PresentationParameters.BackBufferHeight);
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
            //GraphicsDevice.SetRenderTarget(renderTarget);
 
            GraphicsDevice.Clear(gameContext.Background.BackgroundColor);

            gameContext.Root.Draw(spriteBatch);

            //GraphicsDevice.SetRenderTarget(null);

            //spriteBatch.Begin(0, BlendState.Opaque, null, null, null, grayscaleEffect);
            //spriteBatch.Draw((Texture2D)renderTarget, position: Vector2.Zero, color: Color.White);
            //spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}