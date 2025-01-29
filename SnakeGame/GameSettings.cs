using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeGame
{
    public class GameSettings
    {
        public int HighScore { get; set; } = 0;
        public bool AudioMuted { get; set; } = false;
        public int ObstacleFrequency { get; set; } = 20;
        public int StartingObstacles { get; set; } = 50;
    }
}
