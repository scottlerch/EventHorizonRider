using EventHorizonRider.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EventHorizonRider.Core.Components;

internal class Root : ComponentBase
{
    private bool previousPausedPressed;

    public bool PausePressed { get; set; }

    public int? OverrideLevel { get; set; }

    public Space Space { get; private set; }

    public Menu Menu { get; private set; }

    public Foreground Foreground { get; private set; }

    public Music Music { get; private set; }

    public Root(Music music, Space space, Menu menu, Foreground foreground)
        : base(music, space, menu, foreground)
    {
        Music = music;
        Space = space;
        Menu = menu;
        Foreground = foreground;
    }

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

        if (previousPausedPressed && !pausedPressed)
        {
            PausePressed = true;
        }

        previousPausedPressed = pausedPressed;
    }
}
