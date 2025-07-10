using EventHorizonRider.Core.Engine;
using EventHorizonRider.Core.Engine.States;
using EventHorizonRider.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace EventHorizonRider.Core;

public class MainGame : Game
{
    private readonly GraphicsDeviceManager graphicsDeviceManager;

    private InputState inputState;
    private SpriteBatch spriteBatch;
    private GameContext gameContext;

    public MainGame()
    {
        graphicsDeviceManager = new GraphicsDeviceManager(this);
        graphicsDeviceManager.IsFullScreen = DeviceInfo.Platform.IsFullScreen;

        Content.RootDirectory = "Content";

        graphicsDeviceManager.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
    }

    /// <summary>
    /// Event for when game is initialized.  This is useful for development tools to know when to attach to GameContext.
    /// </summary>
    public event EventHandler Initialized = delegate { }; 

    /// <summary>
    /// This exposes the entire game state for use by development tools.
    /// </summary>
    internal GameContext GameContext { get { return gameContext; } }

    /// <remarks>
    /// Only here for testing.  MonoGame should automatically select correct resolution.
    /// </remarks>
    public void SetResolution(int width, int height)
    {
        graphicsDeviceManager.PreferredBackBufferWidth = width;
        graphicsDeviceManager.PreferredBackBufferHeight = height;
    }

    /// <summary>
    /// Allows the game to perform any initialization it needs to before starting to run.
    /// This is where it can query for any required services and load any non-graphic
    /// related content.  Calling base.Initialize will enumerate through any components
    /// and initialize them as well.
    /// </summary>
    protected override void Initialize()
    {
        DeviceInfo.InitializeGraphics(GraphicsDevice);

        IsMouseVisible = DeviceInfo.Platform.IsMouseVisible;
        IsFixedTimeStep = DeviceInfo.Platform.IsFixedTimeStep;
        TargetElapsedTime = DeviceInfo.Platform.TargetElapsedTime;

        gameContext = new GameContext(new InitializeState());
        inputState = new InputState();

        base.Initialize();

        Initialized(this, EventArgs.Empty);
    }

    /// <summary>
    /// LoadContent will be called once per game and is the place to load
    /// all of your content.
    /// </summary>
    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);

        gameContext.Root.LoadContent(Content, GraphicsDevice);
    }

    /// <summary>
    /// UnloadContent will be called once per game and is the place to unload
    /// all content.
    /// </summary>
    protected override void UnloadContent()
    {
        // TODO: Unload any non ContentManager content here
    }

    /// <summary>
    /// Allows the game to run logic such as updating the world,
    /// checking for collisions, gathering input, and playing audio.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Update(GameTime gameTime)
    {
        inputState.Update();
        gameContext.Update(gameTime, inputState);

        base.Update(gameTime);
    }

    /// <summary>
    /// This is called when the game should draw itself.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Draw(GameTime gameTime)
    {
        gameContext.Root.Draw(spriteBatch, GraphicsDevice);

        base.Draw(gameTime);
    }

    /// <summary>
    /// When window/screen/app is activated/deactivate automatically pause game, unless running in windowed moused based environment.
    /// </summary>
    protected override void OnActivated(object sender, EventArgs args)
    {
        if (DeviceInfo.Platform.PauseOnLoseFocus)
        {
            if (gameContext != null)
            {
                gameContext.Paused = true;
            }
        }

        base.OnActivated(sender, args);
    }

    /// <summary>
    /// When window/screen/app is activated/deactivate automatically pause game, unless running in windowed moused based environment.
    /// </summary>
    protected override void OnDeactivated(object sender, EventArgs args)
    {
        if (DeviceInfo.Platform.PauseOnLoseFocus)
        {
            if (gameContext != null)
            {
                gameContext.Paused = true;
            }
        }

        base.OnDeactivated(sender, args);
    }
}