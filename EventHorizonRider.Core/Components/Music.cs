using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace EventHorizonRider.Core.Components
{
    internal class Music : ComponentBase
    {
        private Song musicSong;

        public override void LoadContent(ContentManager content, GraphicsDevice graphics)
        {
#if !WINDOWS
            musicSong = content.Load<Song>("techno_song");
#endif
        }

        public void Play()
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
    }
}