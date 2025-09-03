using EventHorizonRider.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EventHorizonRider.Core.Components;

internal class Root(Music music, Space space, Menu menu, Foreground foreground) : ComponentBase(music, space, menu, foreground)
{
    private bool _previousPausedPressed;

    public bool PausePressed { get; set; }

    public int? OverrideLevel { get; set; }

    public Space Space { get; private set; } = space;

    public Menu Menu { get; private set; } = menu;

    public Foreground Foreground { get; private set; } = foreground;

    public Music Music { get; private set; } = music;

    protected override void UpdateCore(GameTime gameTime, InputState inputState)
    {
        PausePressed = false;

        var keys = inputState.KeyState.GetPressedKeys();
        var pausedPressed = false;

        foreach (var key in keys)
        {
            if (key == Keys.P)
            {
                pausedPressed = true;
            }
        }

        if (_previousPausedPressed && !pausedPressed)
        {
            PausePressed = true;
        }

        _previousPausedPressed = pausedPressed;
    }
}
