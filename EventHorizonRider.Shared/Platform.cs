﻿using System;

namespace EventHorizonRider.Core;

public enum PixelShaderDetail
{
    None,
    Half,
    Full,
}

public enum CollisionDetectionDetail
{
    Half,
    Full,
}

public enum ParticleEffectsDetails
{
    None,
    Full,
}

/// <summary>
/// Platform specific settings like input support (mouse/touch) or graphical detail level supported.
/// </summary>
public class Platform
{
    public bool PauseOnLoseFocus { get; set; }

    public bool IsMouseVisible { get; set; }

    public bool TouchEnabled { get; set; }

    public bool UseDynamicStars { get; set; }

    public bool IsPixelShaderEnabled { get { return PixelShaderDetail != PixelShaderDetail.None; } }

    public PixelShaderDetail PixelShaderDetail { get; set; }

    public CollisionDetectionDetail CollisionDetectionDetail { get; set; }

    public TimeSpan TargetElapsedTime { get; set; }

    public bool IsFixedTimeStep { get; set; }

    public ParticleEffectsDetails ParticleEffectsDetails { get; set; }

    public bool IsFullScreen { get; set; }

    public Platform()
    {
        IsFullScreen = false;
        IsMouseVisible = false;
        UseDynamicStars = false;
        PixelShaderDetail = PixelShaderDetail.Full;
        CollisionDetectionDetail = CollisionDetectionDetail.Full;
        TouchEnabled = true;
        PauseOnLoseFocus = true;
        TargetElapsedTime = TimeSpan.FromSeconds(1/60D);
        IsFixedTimeStep = true;
        ParticleEffectsDetails = ParticleEffectsDetails.Full;
    }
}
