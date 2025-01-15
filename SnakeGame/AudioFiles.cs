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
        private static MediaPlayer LoadAudio(string filename, bool autoReset = true)
        {
            MediaPlayer mp = new MediaPlayer();
            mp.Open(new Uri($"Assets/{filename}", UriKind.Relative));

            if (autoReset)
            {
                mp.MediaEnded += Player_MediaEnded;
            }

            return mp;
        }

        private static void Player_MediaEnded(object? sender, EventArgs e)
        {
            MediaPlayer player = (MediaPlayer)sender;
            player.Stop();
            player.Position = TimeSpan.Zero;
        }
    }
}
