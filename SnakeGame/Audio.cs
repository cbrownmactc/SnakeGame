using System;
using System.Security.Principal;
using System.Threading;
using System.Windows.Markup;
using System.Windows.Media;

namespace SnakeGame
{
    public static class Audio
    {
        public readonly static MediaPlayer Tumbleweed = LoadAudio("tumbleweed.mp3", 0.1, true);
        public readonly static MediaPlayer Bell = LoadAudio("bell.wav", 0.1);
        public readonly static MediaPlayer Dead = LoadAudio("dead.wav", 1);
        public readonly static MediaPlayer Hiss0 = LoadAudio("hiss0.wav", 0.4);
        public readonly static MediaPlayer Hiss1 = LoadAudio("hiss1.wav", 0.4);
        public readonly static MediaPlayer Hiss2 = LoadAudio("hiss2.wav", 0.4);
        public readonly static MediaPlayer WhipCrack = LoadAudio("whip-crack-1.wav", 0.1);
        public readonly static MediaPlayer ClockTick = LoadAudio("clock-ticking.mp3", 0.2);


        /// <summary>
        /// Create a MediaPlayer object for a sound file.
        /// </summary>
        /// <param name="filename">File to load (from Assets/Audio)</param>
        /// <param name="volume">Volume, between 0 and 1</param>
        /// <param name="repeat">Should audio loop</param>
        /// <param name="autoReset">Should audio reposition to 0 when complete</param>
        /// <returns></returns>
        private static MediaPlayer LoadAudio(string filename, double volume, bool repeat = false, bool autoReset = true)
        {
            MediaPlayer player = new MediaPlayer(); 
            player.Open(new Uri($"Assets/Audio/{filename}", UriKind.Relative));
            player.Volume = volume;

            if (autoReset)
                player.MediaEnded += Player_MediaEnded;

            if (repeat)
                player.MediaEnded += PlayerRepeat_MediaEnded;

            
            return player;
        }

        /// <summary>
        /// Method to use if media should play repeatedly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void PlayerRepeat_MediaEnded(object? sender, EventArgs e)
        {
            MediaPlayer m = sender as MediaPlayer;
            m.Play();
        }

        /// <summary>
        /// Method to use if media should reset to 0 when completed playing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Player_MediaEnded(object? sender, EventArgs e)
        {
            MediaPlayer m = sender as MediaPlayer;
            m.Stop();
            m.Position = new TimeSpan(0);
        }
    }
}
