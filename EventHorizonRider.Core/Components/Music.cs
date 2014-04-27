﻿using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace EventHorizonRider.Core.Components
{
    internal class Music : ComponentBase
    {
        private Song musicSong;

        protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
        {
#if !WINDOWS
            musicSong = content.Load<Song>(@"Music\techno_song");
#endif
        }

        public void Start()
        {
#if !WINDOWS
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(musicSong);
#endif
        }

        public void Stop()
        {
#if !WINDOWS
            MediaPlayer.Stop();
#endif
        }

        public void Pause()
        {
#if !WINDOWS
            if (MediaPlayer.State != MediaState.Paused)
            {
                MediaPlayer.Pause();
            }
#endif
        }

        public void Play()
        {
#if !WINDOWS
            if (MediaPlayer.State != MediaState.Playing)
            {
                MediaPlayer.Resume();
            }
#endif
        }
    }
}