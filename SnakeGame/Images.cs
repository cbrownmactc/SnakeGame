using System;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SnakeGame
{
    public static class Images
    {
        public readonly static ImageSource Empty = LoadImage("Empty.png");
        public readonly static ImageSource Body = LoadImage("Body.png");
        public readonly static ImageSource Head = LoadImage("Head.png");
        public readonly static ImageSource Food = LoadImage("Food.png");
        public readonly static ImageSource DeadBody = LoadImage("DeadBody.png");
        public readonly static ImageSource DeadHead = LoadImage("DeadHead.png");
        public readonly static ImageSource SpeakerOn = LoadImage("SpeakerIconGreen.png");
        public readonly static ImageSource SpeakerOff = LoadImage("SpeakerIconGray.png");
        public readonly static ImageSource SpeedOn = LoadImage("SpeedIconGreen.png");
        public readonly static ImageSource SpeedOff = LoadImage("SpeedIconGray.png");

        private static ImageSource LoadImage(string filename)
        {
            return new BitmapImage(new Uri($"Assets/Images/{filename}", UriKind.Relative));
        }
    }
}
