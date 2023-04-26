using System;
using System.Windows.Markup;
using System.Windows.Media;

namespace SnakeGame
{
    public static class Audio
    {
        public readonly static MediaPlayer Tumbleweed = LoadAudio("tumbleweed.mp3");

        private static MediaPlayer LoadAudio(string filename)
        {
            MediaPlayer player = new MediaPlayer();
            player.Open(new Uri($"Assets/Audio/{filename}", UriKind.Relative));
            player.Volume = 0.1;
            return player;
        }
    }
}
