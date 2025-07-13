using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace EventHorizonRider.Core.Components;

internal class Music : ComponentBase
{
    private Song musicSong;

    protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
    {
        musicSong = content.Load<Song>(@"Music\techno_song");
    }

    public void Start()
    {
        Microsoft.Xna.Framework.Media.MediaPlayer.IsRepeating = true;
        Microsoft.Xna.Framework.Media.MediaPlayer.Play(musicSong);
    }

    public void Stop()
    {
        Microsoft.Xna.Framework.Media.MediaPlayer.Stop();
    }

    public void Pause()
    {
        Updating = false;

        if (Microsoft.Xna.Framework.Media.MediaPlayer.State != MediaState.Paused)
        {
            Microsoft.Xna.Framework.Media.MediaPlayer.Pause();
        }
    }

    public void Play()
    {
        Updating = true;

        if (Microsoft.Xna.Framework.Media.MediaPlayer.State != MediaState.Playing)
        {
            Microsoft.Xna.Framework.Media.MediaPlayer.Resume();
        }
    }
}