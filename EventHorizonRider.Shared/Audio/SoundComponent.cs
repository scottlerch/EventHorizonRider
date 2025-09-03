using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace EventHorizonRider.Core.Audio;

internal class SoundComponent
{
    private readonly SoundEffectInstance _soundEffectInstance;

    private float _currentFadeSpeed;

    public SoundComponent(SoundEffect soundEffect)
    {
        _soundEffectInstance = soundEffect.CreateInstance();
        _soundEffectInstance.IsLooped = true;
        _soundEffectInstance.Volume = 0f;

        MaxVolume = 1f;
        MinVolume = 0f;

        FadeSpeed = 1f;
        _currentFadeSpeed = 0f;
    }

    public bool Paused { get; set; }

    public float FadeSpeed { get; set; }

    public float MaxVolume { get; set; }

    public float MinVolume { get; set; }

    public void PlayMax(bool immediate = false)
    {
        Paused = false;

        _currentFadeSpeed = FadeSpeed;

        if (immediate)
        {
            _soundEffectInstance.Volume = MaxVolume;
            UpdateState();
        }
    }

    public void PlayMin(bool immediate = false)
    {
        Paused = false;

        _currentFadeSpeed = -FadeSpeed;

        if (immediate)
        {
            _soundEffectInstance.Volume = MinVolume;
            UpdateState();
        }
    }

    public void Pause()
    {
        Paused = true;

        if (_soundEffectInstance.State == SoundState.Playing)
        {
            _soundEffectInstance.Pause();
        }
    }

    public void Update(GameTime gameTime)
    {
        if (Paused)
        {
            return;
        }

        if (_soundEffectInstance.State == SoundState.Paused)
        {
            _soundEffectInstance.Resume();
        }

        var volume = _soundEffectInstance.Volume + (float)gameTime.ElapsedGameTime.TotalSeconds * _currentFadeSpeed;

        if (volume >= MaxVolume)
        {
            volume = MaxVolume;
        }
        else if (volume <= MinVolume)
        {
            volume = MinVolume;
        }

        _soundEffectInstance.Volume = volume;

        UpdateState();
    }

    private void UpdateState()
    {
        if (_soundEffectInstance.Volume > 0 && _soundEffectInstance.State != SoundState.Playing)
        {
            _soundEffectInstance.Play();
        }
        else if (_soundEffectInstance.Volume <= 0 && _soundEffectInstance.State != SoundState.Stopped)
        {
            _soundEffectInstance.Stop();
        }
    }
}
