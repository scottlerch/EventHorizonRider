using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace EventHorizonRider.Core.Components;

internal class Music : ComponentBase
{
    private Song _musicSong;

    protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics) => _musicSong = content.Load<Song>(@"Music\techno_song");

    public void Start()
    {
        MediaPlayer.IsRepeating = true;
        MediaPlayer.Play(_musicSong);
    }

    public void Stop() => MediaPlayer.Stop();

    public void Pause()
    {
        Updating = false;

        if (MediaPlayer.State != MediaState.Paused)
        {
            MediaPlayer.Pause();
        }
    }

    public void Play()
    {
        Updating = true;

        if (MediaPlayer.State != MediaState.Playing)
        {
            MediaPlayer.Resume();
        }
    }
}
