using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace SnakeGame
{
    public class AudioFiles
    {
        public static readonly MediaPlayer EatFood = LoadAudio("beep.wav");
        public static readonly MediaPlayer GameOver = LoadAudio("game-over.wav");
        private static MediaPlayer LoadAudio(string filename, 
            bool autoReset = true,
            bool autoRepeat = false)
        {
            MediaPlayer mp = new MediaPlayer();
            mp.Open(new Uri($"Assets/{filename}", UriKind.Relative));

            if (autoReset)
            {
                mp.MediaEnded += Player_MediaEnded;
            }

            if (autoRepeat)
            {
                mp.MediaEnded += Player_RepeatMediaEnded;
            }

            return mp;
        }

        private static void Player_RepeatMediaEnded(object? sender, EventArgs e)
        {
            MediaPlayer player = (MediaPlayer)sender;
            player.Stop();
            player.Position = TimeSpan.Zero;
            player.Play();
        }

        private static void Player_MediaEnded(object? sender, EventArgs e)
        {
            MediaPlayer player = (MediaPlayer)sender;
            player.Stop();
            player.Position = TimeSpan.Zero;
        }
    }
}
