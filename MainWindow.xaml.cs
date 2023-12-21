using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace AMSnake
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Dictionary<GridValue, ImageSource>
            gridValToImage = new()
            {
                { GridValue.Empty, Images.Empty },
                { GridValue.Snake, Images.Body },
                {GridValue.Food , Images.Food },
                {GridValue.Wall , Images.Wall }
            };

        private readonly Dictionary<Direction, int>
            dirToRotation = new()
            {
                { Direction.Up, 0 },
                { Direction.Right, 90 },
                { Direction.Down, 180 },
                { Direction.Left, 270 },
            };
        private readonly int rows = 15;
        private readonly int cols = 15;
        private readonly Image[,] gridImages;
        private GameState gameState;
        private bool gameRunning;
        private int boostSpeed = 0;
        int delay = 100;
        private Random random = new Random();

        public MainWindow()
        {
            InitializeComponent();
            EndGameText.Visibility = Visibility.Hidden;
            gridImages = SetupGrid();
            gameState = new GameState(rows, cols);
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
                        RenderTransformOrigin = new Point(.5, .5)
                    };
                    images[r, c] = image;
                    GameGrid.Children.Add(image);
                }
            }

            return images;
        }

        private async Task RunGame()
        {
            Draw();
            await ShowCountDown();
             if (GameSettings.EnableBGMusic) Audio.BGMusic[random.Next(0, Audio.BGMusic.Count)].Play();
            Overlay.Visibility = Visibility.Hidden;
            await GameLoop();
            await ShowGameOver();
            gameState = new GameState(rows, cols);
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
                case Key.Space:
                    boostSpeed = (boostSpeed == 0) ? GameSettings.BoostSpeed : 0;
                    break;
            }
        }

        private async Task GameLoop()
        {
            while (!gameState.GameOver)
            {
                await Task.Delay(delay - boostSpeed);
                gameState.Move();
                Draw();
            }
        }


        private void Draw()
        {
            DrawGrid();
            DrawSnakeHead();
            ScoreText.Text = $"Score {gameState.Score}";
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

        private async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
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



        private async Task ShowCountDown()
        {
            for (int i = 3; i >= 1; i--)
            {
                EndGameText.Visibility = Visibility.Hidden;
                OverlayText.Text = i.ToString();
                await Task.Delay(500);
            }
        }

        private async Task ShowGameOver()
        {
            boostSpeed = 0;
            ShakeWindow(GameSettings.ShakeDuration);
            foreach (MediaPlayer m in Audio.BGMusic)
            {
                m.Stop();
            }
            EndGameText.Text = "Game Over!";
            EndGameText.Visibility = Visibility.Visible;
            Audio.GameOver.Play();
            await DrawDeadSnake();
            await Task.Delay(1000);
            OverlayText.Text = "Press Any Key To Start";
            Overlay.Visibility = Visibility.Visible;
        }

        private async Task DrawSnakeHead()
        {
            Position headPos = gameState.HeadPosition();
            Image image = gridImages[headPos.Row, headPos.Col];
            image.Source = Images.Head;

            int rotation = dirToRotation[gameState.Dir];
            image.RenderTransform = new RotateTransform(rotation);
        }

        private async Task DrawDeadSnake()
        {
            List<Position> positions = new List<Position>(gameState.SnakePositions());

            for (int i = 0; i < positions.Count; i++)
            {
                Position pos = positions[i];
                ImageSource source = (i == 0) ? Images.DeadHead : Images.DeadBody;
                gridImages[pos.Row, pos.Col].Source = source;
                await Task.Delay(Math.Max(50-(i*3), 10));
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

            shakeTimer.Interval = TimeSpan.FromMilliseconds(200);
            shakeTimer.Start();

            await Task.Delay(durationMs);
            shakeTimer.Stop();
        }

        private void OpenWindow(object sender, RoutedEventArgs e)
        {
            GameSettings1 gameSettings1 = new GameSettings1(this);
            this.Visibility = Visibility.Hidden;
            gameSettings1.Show();
            WindowState = WindowState.Maximized;
        }
    }
}


