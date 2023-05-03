using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeGame
{
    internal static class GameSettings
    {
        public static int DefaultRows { get; set; } = 20;
        public static int DefaultCols { get; set; } = 20;
        public static double SoundFxFrequency = 0.03;
        public static int FrameFrequency { get; set; } = 100;
        public static int SpeedAdjustment { get; set; } = 2;
    }
}
