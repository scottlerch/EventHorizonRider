using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace EventHorizonRider.Core.Components;

internal class Music : ComponentBase
{
    private Song musicSong;

    protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
    {
#if !PSM
        musicSong = content.Load<Song>(@"Music\techno_song");
#endif
    }

    public void Start()
    {
#if !PSM
        Microsoft.Xna.Framework.Media.MediaPlayer.IsRepeating = true;
        Microsoft.Xna.Framework.Media.MediaPlayer.Play(musicSong);
#endif
    }

    public void Stop()
    {
#if !PSM
        Microsoft.Xna.Framework.Media.MediaPlayer.Stop();
#endif
    }

    public void Pause()
    {
#if !PSM
        Updating = false;

        if (Microsoft.Xna.Framework.Media.MediaPlayer.State != MediaState.Paused)
        {
            Microsoft.Xna.Framework.Media.MediaPlayer.Pause();
        }
#endif
    }

    public void Play()
    {
#if !PSM
        Updating = true;

        if (Microsoft.Xna.Framework.Media.MediaPlayer.State != MediaState.Playing)
        {
            Microsoft.Xna.Framework.Media.MediaPlayer.Resume();
        }
#endif
    }
}