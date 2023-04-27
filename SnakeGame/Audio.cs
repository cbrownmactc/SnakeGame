using System;
using System.Security.Principal;
using System.Windows.Markup;
using System.Windows.Media;

namespace SnakeGame
{
    public static class Audio
    {
        public readonly static MediaPlayer Tumbleweed = LoadAudio("tumbleweed.mp3", 0.1, true);
        public readonly static MediaPlayer Bell = LoadAudio("bell.wav", 0.1);
        public readonly static MediaPlayer Dead = LoadAudio("dead.wav", 1);
        public readonly static MediaPlayer Hiss0 = LoadAudio("hiss0.wav", 0.1);
        public readonly static MediaPlayer Hiss1 = LoadAudio("hiss1.wav", 0.1);
        public readonly static MediaPlayer Hiss2 = LoadAudio("hiss2.wav", 0.1);


        private static MediaPlayer LoadAudio(string filename, double volume, bool repeat = false)
        {
            MediaPlayer player = new MediaPlayer(); 
            player.Open(new Uri($"Assets/Audio/{filename}", UriKind.Relative));
            player.Volume = volume;

            if (repeat)
                player.MediaEnded += Player_MediaEnded;
            return player;
        }

        private static void Player_MediaEnded(object? sender, EventArgs e)
        {
            MediaPlayer m = sender as MediaPlayer;
            m.Position = new TimeSpan(0);
            m.Play();
        }
    }
}
