using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SnakeGame
{
    internal class GameState
    {
        public int Rows { get; }
        public int Cols { get; }
        public GridValue[,] Grid { get; }
        public Direction Dir { get; private set; }
        public int Score { get; private set; }
        public int FruitCount { get; private set; }
        public bool GameOver { get; private set; }
        public int CurrentFrequency { get; set; }
        public bool AdjustableSpeed { get; set; }

        public int TotalMoves { get; set;  }

        private readonly LinkedList<Direction> dirChanges = new LinkedList<Direction>();
        private readonly LinkedList<Position> snakePositions = new LinkedList<Position>();
        private readonly Random random = new Random();

        public GameState(int rows, int cols, int frameFrequency, bool adjustableSpeed)
        {
            Rows = rows;
            Cols = cols;
            Grid = new GridValue[rows, cols];
            Dir = Direction.Right;
            CurrentFrequency = GameSettings.FrameFrequency;
            AdjustableSpeed = adjustableSpeed;

            AddSnake();
            AddFood();
        }

        private void AddFood()
        {
            List<Position> empty = new List<Position>(EmptyPositions());

            if (empty.Count == 0)
            {
                return;
            }

            Position pos = empty[random.Next(empty.Count)];
            Grid[pos.Row, pos.Col] = GridValue.Food;
        }

        private void AddHead(Position pos)
        {
            snakePositions.AddFirst(pos);
            Grid[pos.Row, pos.Col] = GridValue.Snake;
        }

        private void AdjustSpeed()
        {
            if (AdjustableSpeed)
            {
                // For now, decrease frequency every 50 moves, minimum of 20
                //CurrentFrequency = GameSettings.FrameFrequency - (TotalMoves / 50);

                // Or, reduce frequency for each fruit we've got
                CurrentFrequency = GameSettings.FrameFrequency - (FruitCount * GameSettings.SpeedAdjustment);

                CurrentFrequency = Math.Max(CurrentFrequency, 20);

            }
        }

        private void AddSnake()
        {
            int r = Rows / 2;

            for (int c=1; c <= 3;  c++)
            {
                Grid[r, c] = GridValue.Snake;
                snakePositions.AddFirst(new Position(r, c));
            }
        }

        private bool CanChangeDirection(Direction newDir)
        {
            if (dirChanges.Count == 2)
            {
                return false;
            }

            // This really needs a comment
            Direction lastDir = GetLastDirection();
            return newDir != lastDir && newDir != lastDir.Opposite();

        }

        public void ChangeDirection(Direction dir)
        {
            if (CanChangeDirection(dir))
            {
                dirChanges.AddLast(dir);
            }
        }

        private IEnumerable<Position> EmptyPositions()
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (Grid[r,c] == GridValue.Empty)
                    {
                        yield return new Position(r, c);
                    }
                }
            }
        }

        private Direction GetLastDirection()
        {
            if (dirChanges.Count == 0)
            { return Dir; }

            return dirChanges.Last.Value;
        }

        public Position HeadPosition()
        {
            return snakePositions.First.Value;
        }

        public void Move()
        {
            TotalMoves++;
            AdjustSpeed();

            if (dirChanges.Count > 0)
            {
                Dir = dirChanges.First.Value;
                dirChanges.RemoveFirst();
            }
            Position newHeadPos = HeadPosition().Translate(Dir);
            GridValue hit = WillHit(newHeadPos);

            if (hit == GridValue.Outside || hit == GridValue.Snake)
            {
                GameOver = true;
            }
            else if (hit == GridValue.Empty)
            {
                RemoveTail();
                AddHead(newHeadPos);
            }
            else if (hit == GridValue.Food)
            {
                AddHead(newHeadPos);
                FruitCount++;

                // If adjustable speed, score should go up faster at higher levels
                if (AdjustableSpeed)
                {
                    Score += 1 + (TotalMoves / 50);
                }
                else
                {
                    Score++;
                }


                // This is succinct, but seems like there might be a cleaner way
                new List<MediaPlayer>() { Audio.Bell, Audio.WhipCrack }[random.Next(0, 2)].Play();

                AddFood();
            }
        }

        private bool OutsideGrid(Position pos)
        {
            return pos.Row < 0 || pos.Row >= Rows || pos.Col < 0 || pos.Col >= Cols;
        }

        private void RemoveTail()
        {
            Position tail = snakePositions.Last.Value;
            Grid[tail.Row, tail.Col] = GridValue.Empty;
            snakePositions.RemoveLast();
        }

        public IEnumerable<Position> SnakePositions()
        {
            return snakePositions;
        }

        public Position TailPosition()
        {
            return snakePositions.Last.Value;
        }

        private GridValue WillHit(Position newHeadPos)
        {
            if (OutsideGrid(newHeadPos))
            {
                return GridValue.Outside;
            }

            if (newHeadPos == TailPosition())
            {
                return GridValue.Empty;
            }

            return Grid[newHeadPos.Row, newHeadPos.Col];
        }

    }
}
