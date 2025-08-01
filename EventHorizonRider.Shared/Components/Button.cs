﻿using EventHorizonRider.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Linq;

namespace EventHorizonRider.Core.Components;

internal class Button(Rectangle buttonBounds, Keys? key, TimeSpan holdDuration)
{
    public Rectangle ButtonBounds { get; private set; } = buttonBounds;

    public Keys? Key { get; private set; } = key;

    private bool keyPreviouslyPressed;
    private bool mousePreviouslyPressed;

    public bool Pressed { get; private set; }

    public bool Hover { get; private set; }

    public TimeSpan HoldDuration { get; set; } = holdDuration;

    public TimeSpan CurrentHoldDuration { get; private set; }

    public TimeSpan HoldDurationRemaining
    {
        get
        {
            return CurrentHoldDuration >= HoldDuration? TimeSpan.Zero : HoldDuration - CurrentHoldDuration;
        }
    }

    public bool Holding { get { return CurrentHoldDuration > TimeSpan.Zero; } }

    public Button(Rectangle buttonBounds, Keys? key = null)
        : this(buttonBounds, key, TimeSpan.Zero)
    {
    }

    public void Update(GameTime gameTime, InputState inputState, bool visible)
    {
        if (!visible)
        {
            Pressed = false;
            Hover = false;
            return;
        }

        Pressed = !Pressed && IsPressed(inputState.MouseState, inputState.TouchState, inputState.KeyState);
        Hover = IsHover(inputState.MouseState, inputState.TouchState, inputState.KeyState);

        keyPreviouslyPressed = Key.HasValue && inputState.KeyState.GetPressedKeys().Contains(Key.Value);

        mousePreviouslyPressed = 
            inputState.MouseState.LeftButton == ButtonState.Pressed && 
            ButtonBounds.Contains(inputState.MouseState.Position);

        if (HoldDuration > TimeSpan.Zero)
        {
            var isPressed = Pressed;
            Pressed = false;

            if (Hover)
            {
                CurrentHoldDuration += gameTime.ElapsedGameTime;
            }
            else
            {
                if (CurrentHoldDuration > HoldDuration)
                {
                    Pressed = isPressed;
                }

                CurrentHoldDuration = TimeSpan.Zero;
            }
        }
    }

    private bool IsPressed(MouseState mouseState, TouchCollection touchState, KeyboardState keyboardState)
    {
        return
            IsPressed(keyboardState) ||
            IsPressed(mouseState) ||
            IsPressed(touchState);
    }

    private bool IsHover(MouseState mouseState, TouchCollection touchState, KeyboardState keyboardState)
    {
        return
            IsHover(keyboardState) ||
            IsHover(mouseState) ||
            IsHover(touchState);
    }

    private bool IsHover(MouseState mouseState)
    {
        return
            mousePreviouslyPressed &&
            mouseState.LeftButton == ButtonState.Pressed &&
            ButtonBounds.Contains(mouseState.Position);
    }

    private bool IsHover(KeyboardState keyboardState)
    {
        return Key.HasValue && keyboardState.GetPressedKeys().Contains(Key.Value);
    }

    private bool IsHover(TouchCollection touchState)
    {
        return touchState.Any(t => 
            (
                t.State == TouchLocationState.Pressed ||
                t.State == TouchLocationState.Moved
            ) && ButtonBounds.Contains(t.Position));
    }

    private bool IsPressed(MouseState mouseState)
    {
        return mousePreviouslyPressed &&
               mouseState.LeftButton == ButtonState.Released &&
               ButtonBounds.Contains(mouseState.Position);
    }

    private bool IsPressed(KeyboardState keyboardState)
    {
        return Key.HasValue && 
            keyPreviouslyPressed && 
            !keyboardState.GetPressedKeys().Contains(Key.Value);
    }

    private bool IsPressed(TouchCollection touchState)
    {
        return touchState.Any(t => 
            t.State == TouchLocationState.Released &&
            ButtonBounds.Contains(t.Position));
    }
}
