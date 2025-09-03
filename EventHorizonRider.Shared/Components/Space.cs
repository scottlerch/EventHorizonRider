using EventHorizonRider.Core.Components.SpaceComponents;
using EventHorizonRider.Core.Graphics;
using EventHorizonRider.Core.Input;
using EventHorizonRider.Core.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace EventHorizonRider.Core.Components;

internal class Space : ComponentBase
{
    private RenderTarget2D _renderTarget1;
    private RenderTarget2D _renderTarget2;
    private readonly GaussianBlur _blur = new();
    private Color _color;

    private readonly Motion _blurAmountMotion;

    public Color Color
    {
        get => _color;
        set
        {
            _color = value;
            Ship.Color = value;
            Ship.Shield.Color = value;
            Background.StarBackgroundColor = value;
            Rings.Color = value;
        }
    }

    public Blackhole Blackhole { get; private set; }

    public BlackholeHalo BlackholeHalo { get; private set; }

    public RingCollection Rings { get; private set; }

    public Ship Ship { get; private set; }

    public Background Background { get; private set; }

    public Shockwave Shockwave { get; private set; }

    public bool BlurEnabled { get; set; }

    public void SetBlur(float blurAmount)
    {
        if (BlurEnabled)
        {
            _blurAmountMotion.Set(blurAmount);
            _blur.BlueAmount = _blurAmountMotion.Value * DeviceInfo.OutputScale;
        }
    }

    public void StartBlur(float blurAmount, float speed)
    {
        if (BlurEnabled)
        {
            _blurAmountMotion.UpdateTarget(blurAmount, speed);
        }
    }

    public void StopBlur()
    {
        if (BlurEnabled)
        {
            _blurAmountMotion.UpdateTarget(0);
        }
    }

    public Space(Background background, BlackholeHalo blackholeHalo, Shockwave shockwave, RingCollection ringCollection, Ship ship, Blackhole blackhole)
        : base(background, blackholeHalo, shockwave, ship, ringCollection, blackhole)
    {
        BlurEnabled = DeviceInfo.Platform.IsPixelShaderEnabled;

        _blurAmountMotion = new Motion();
        _blurAmountMotion.Initialize(0, 0, 30);

        Background = background;
        BlackholeHalo = blackholeHalo;
        Shockwave = shockwave;
        Rings = ringCollection;
        Ship = ship;
        Blackhole = blackhole;
    }

    protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
    {
        if (BlurEnabled)
        {
            _renderTarget1 = new RenderTarget2D(
                graphics,
                graphics.PresentationParameters.BackBufferWidth,
                graphics.PresentationParameters.BackBufferHeight);

            var scaleBackBuffer = 2f;

            if (DeviceInfo.Platform.PixelShaderDetail == PixelShaderDetail.Half)
            {
                scaleBackBuffer = 4f;
            }

            _renderTarget2 = new RenderTarget2D(
                graphics,
                (int)Math.Round((graphics.PresentationParameters.BackBufferWidth / scaleBackBuffer) * DeviceInfo.InputScale),
                (int)Math.Round((graphics.PresentationParameters.BackBufferHeight / scaleBackBuffer) * DeviceInfo.InputScale));

            _blur.LoadContent(content);
        }
    }

    protected override void UpdateCore(GameTime gameTime, InputState inputState)
    {
        _blurAmountMotion.Update(gameTime);
        _blur.BlueAmount = _blurAmountMotion.Value * DeviceInfo.OutputScale;
    }

    protected override void OnBeforeDraw(SpriteBatch spriteBatch, GraphicsDevice graphics)
    {
        if (BlurEnabled)
        {
            if (_blurAmountMotion.Value > 0)
            {
                graphics.SetRenderTarget(_renderTarget1);
            }
        }

        graphics.Clear(Background.BackgroundColor);

        spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, DeviceInfo.OutputScaleMatrix);
    }

    protected override void OnAfterDraw(SpriteBatch spriteBatch, GraphicsDevice graphics)
    {
        spriteBatch.End();

        if (BlurEnabled)
        {
            if (_blurAmountMotion.Value > 0)
            {
                graphics.SetRenderTarget(_renderTarget2);

                _blur.SetBlurEffectParameters(1f / _renderTarget2.Width, 0f);
                spriteBatch.Begin(0, BlendState.Opaque, null, null, null, _blur.Effect);
                spriteBatch.Draw(_renderTarget1, _renderTarget2.Bounds, Color.White);
                spriteBatch.End();

                graphics.SetRenderTarget(null);

                _blur.SetBlurEffectParameters(0f, 1f / _renderTarget1.Height);
                spriteBatch.Begin(0, BlendState.Opaque, null, null, null, _blur.Effect);
                spriteBatch.Draw(_renderTarget2, _renderTarget1.Bounds, Color.White);
                spriteBatch.End();
            }
        }
    }
}
