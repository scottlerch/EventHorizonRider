using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace EventHorizonRider.Core.Audio
{
    internal class SoundComponent
    {
        private readonly SoundEffectInstance soundEffectInstance;

        private float currentFadeSpeed;

        public SoundComponent(SoundEffect soundEffect)
        {
            soundEffectInstance = soundEffect.CreateInstance();
            soundEffectInstance.IsLooped = true;
            soundEffectInstance.Volume = 0f;

            MaxVolume = 1f;
            MinVolume = 0f;

            FadeSpeed = 1f;
            currentFadeSpeed = 0f;
        }

        public bool Paused { get; set; }

        public float FadeSpeed { get; set; }

        public float MaxVolume { get; set; }

        public float MinVolume { get; set; }

        public void PlayMax(bool immediate = false)
        {
            Paused = false;

            currentFadeSpeed = FadeSpeed;

            if (immediate)
            {
                soundEffectInstance.Volume = MaxVolume;
                UpdateState();
            }
        }

        public void PlayMin(bool immediate = false)
        {
            Paused = false;

            currentFadeSpeed = -FadeSpeed;

            if (immediate)
            {
                soundEffectInstance.Volume = MinVolume;
                UpdateState();
            }
        }

        public void Pause()
        {
            Paused = true;

            if (soundEffectInstance.State == SoundState.Playing)
            {
                soundEffectInstance.Pause();
            }
        }

        public void Update(GameTime gameTime)
        {
            if (Paused) return;

            if (soundEffectInstance.State == SoundState.Paused)
            {
                soundEffectInstance.Resume();
            }

            var volume =  soundEffectInstance.Volume + (float)gameTime.ElapsedGameTime.TotalSeconds * currentFadeSpeed;

            if (volume >= MaxVolume)
            {
                volume = MaxVolume;
            }
            else if (volume <= MinVolume)
            {
                volume = MinVolume;
            }

            soundEffectInstance.Volume = volume;

            UpdateState();
        }

        private void UpdateState()
        {
            if (soundEffectInstance.Volume > 0 && soundEffectInstance.State != SoundState.Playing)
            {
                soundEffectInstance.Play();
            }
            else if (soundEffectInstance.Volume <= 0 && soundEffectInstance.State != SoundState.Stopped)
            {
                soundEffectInstance.Stop();
            }
        }
    }
}
