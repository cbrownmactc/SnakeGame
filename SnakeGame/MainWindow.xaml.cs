using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SnakeGame
{

    public partial class MainWindow : Window
    {
        private readonly Dictionary<GridValue, ImageSource> gridValToImage = new()
        {
            { GridValue.Empty, Images.Empty },
            { GridValue.Snake, Images.Body },
            { GridValue.Food, Images.Food }
        };

        private readonly Dictionary<Direction, int> dirToRotation = new Dictionary<Direction, int>()
        {
            { Direction.Up, 0 },
            { Direction.Right, 90 },
            { Direction.Down, 180 },
            { Direction.Left, 270 }
        };

        private readonly int rows = GameSettings.DefaultRows, cols = GameSettings.DefaultCols;
        private GameState gameState;
        private bool gameRunning = false;

        private readonly Image[,] gridImages;
        private MediaPlayer backgroundAudio = Audio.Tumbleweed;
        private Random random = new Random();

        // Game play settings
        private bool backgroundPlaying = true;
        private bool speedOn = true;
        private bool shakeOn = true;

        public MainWindow()
        {
            InitializeComponent();
            //backgroundAudio.Play();
            gridImages = SetupGrid();
            gameState = new GameState(rows, cols, GameSettings.FrameFrequency, true);
        }

        private void Draw()
        {
            DrawGrid();
            DrawSnakeHead();
            ScoreText.Text = $"SCORE {gameState.Score} [{gameState.TotalMoves}]";
        }

        private async Task DrawDeadSnake()
        {
            List<Position> positions = new List<Position>(gameState.SnakePositions());

            for (int i = 0; i < positions.Count; i++)
            {
                Position pos = positions[i];
                ImageSource source = (i == 0) ? Images.DeadHead : Images.DeadBody;
                gridImages[pos.Row, pos.Col].Source = source;
                await Task.Delay(Math.Max(10, 50-i));
            }
        }

        private void DrawGrid()
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    GridValue gridVal = gameState.Grid[r, c];
                    gridImages[r, c].Source = gridValToImage[gridVal];
                    gridImages[r, c].RenderTransform = Transform.Identity;
                }
            }
        }

        private void DrawSnakeHead()
        {
            Position headPos = gameState.HeadPosition();
            Image image = gridImages[headPos.Row, headPos.Col];
            image.Source = Images.Head;

            int rotation = dirToRotation[gameState.Dir];
            image.RenderTransform = new RotateTransform(rotation);
        }

        private async Task GameLoop()
        {
            while (!gameState.GameOver)
            {
                await Task.Delay(gameState.CurrentFrequency);
                gameState.Move();
                PlaySoundFx();
                Draw();
            }
        }

        private void MenuBackgroundSound_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ToggleBackgroundAudio();
        }

        private void MenuSpeed_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ToggleVariableSpeed();
        }

        private void PlaySoundFx()
        {
            if (random.NextDouble() < GameSettings.SoundFxFrequency)
            {
                new List<MediaPlayer>() { Audio.Hiss0, Audio.Hiss1, Audio.Hiss2 }[random.Next(0, 3)].Play();
            }
        }

        private async Task RunGame()
        {
            Draw();
            await ShowCountDown();
            Overlay.Visibility = Visibility.Hidden;

            if (backgroundPlaying)
                backgroundAudio.Play();

            await GameLoop();
            await ShowGameOver();
            gameState = new GameState(rows, cols, GameSettings.FrameFrequency, speedOn);
        }

        private Image[,] SetupGrid()
        {
            Image[,] images = new Image[rows, cols];
            GameGrid.Rows = rows;
            GameGrid.Columns = cols;
            GameGrid.Width = GameGrid.Height * (cols / (double)rows);

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Image image = new Image
                    {
                        Source = Images.Empty,
                        RenderTransformOrigin = new Point(0.5, 0.5)
                    };

                    images[r, c] = image;
                    GameGrid.Children.Add(image);
                }
            }

            return images;
        }

        private async Task ShowCountDown()
        {
            for (int i = 3; i >= 1; i--)
            {
                Audio.ClockTick.Play();
                OverlayText.Text = i.ToString();

                // We'll wait for as long as the tick audio is.
                // This is to fix an error that cropped up about Duration being "Automatic" and not a TimeSpan.
                if (Audio.ClockTick.NaturalDuration.HasTimeSpan)
                    await Task.Delay(Convert.ToInt32(Audio.ClockTick.NaturalDuration.TimeSpan.TotalMilliseconds));
                else
                    await Task.Delay(1000);
            }
        }

        private async Task ShowGameOver()
        {
            backgroundAudio.Stop();
            backgroundAudio.Position = new TimeSpan(0);

            Audio.Dead.Play();
            if (shakeOn) ShakeWindow(GameSettings.ShakeDuration);

            await DrawDeadSnake();
            await Task.Delay(1000);
            Overlay.Visibility = Visibility.Visible;
            OverlayText.Text = "Press any key to start";
        }

        private void ToggleBackgroundAudio()
        {
            if (backgroundPlaying)
            {
                backgroundPlaying = false;
                backgroundAudio.Pause();
                MenuBackgroundSound.Source = Images.SpeakerOff;
            }
            else
            {
                backgroundPlaying = true;
                backgroundAudio.Play();
                MenuBackgroundSound.Source = Images.SpeakerOn;
            }
        }

        private void ToggleVariableSpeed()
        {
            // Only show if the overlay is up (NOT mid-game)
            if (Overlay.Visibility == Visibility.Visible)
            {
                if (speedOn)
                {
                    speedOn = false;
                    MenuSpeed.Source = Images.SpeedOff;
                }
                else
                {
                    speedOn = true;
                    MenuSpeed.Source = Images.SpeedOn;
                }

                gameState.AdjustableSpeed = speedOn;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState.GameOver)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Left:
                    gameState.ChangeDirection(Direction.Left);
                    break;
                case Key.Right:
                    gameState.ChangeDirection(Direction.Right);
                    break;
                case Key.Up:
                    gameState.ChangeDirection(Direction.Up);
                    break;
                case Key.Down:
                    gameState.ChangeDirection(Direction.Down);
                    break;
            }
        }

        private async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Handle menu hot keys that should work any time and not trigger game start
            if (e.Key == Key.Space)
            {
                ToggleBackgroundAudio();
                e.Handled = true;
                return;
            }

            if (Overlay.Visibility == Visibility.Visible)
            {
                e.Handled = true;
            }

            if (!gameRunning)
            {
                gameRunning = true;
                await RunGame();
                gameRunning = false;
            }
        }

        private async Task ShakeWindow(int durationMs)
        {
            var oLeft = this.Left;
            var oTop = this.Top;
            var shakeTimer = new DispatcherTimer(DispatcherPriority.Send);

            shakeTimer.Tick += (sender, args) =>
            {
                this.Left = oLeft + random.Next(-10, 11);
                this.Top = oTop + random.Next(-10, 11);
            };

            shakeTimer.Interval = TimeSpan.FromMilliseconds(100);
            shakeTimer.Start();

            await Task.Delay(durationMs);
            shakeTimer.Stop();
        }
    }
}
