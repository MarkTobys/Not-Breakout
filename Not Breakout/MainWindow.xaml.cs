using System.Numerics;
using System.Text;
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

namespace Not_Breakout
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // paddle and ball objects/images
        private Paddle paddle;
        private Ball ball;
        private Image paddleImage;
        private Image ballImage;
        // tick counter for the game
        private DispatcherTimer gameTimer;
        // Play area (canvas) constants;
        public const int gameWidth = 840;
        public const int gameHeight = 700;
        // paddle size constants
        public const int paddleWidth = 100;
        public const int paddleHeight = 20;
        // ball constant (width and height are the same size)
        public const int ballSize = 26;
        public MainWindow()
        {
            InitializeComponent();
            // add the paddle and ball to the play field
            paddle = new Paddle(420, 600);
            paddleImage = new Image
            {
                Source = Images.Paddle,
                Width = paddleWidth,
                Height = paddleHeight
            };
            ball = new Ball(0, 0, 0, paddle.YCoords - ballSize, false);

            ballImage = new Image
            {
                Source = Images.Ball,
                Width = ballSize,
                Height = ballSize
            };
            // Draw the paddle and ball to the screen
            GameCanvas.Children.Add(paddleImage);
            GameCanvas.Children.Add(ballImage);
            // Set up game timer to run at 120 times per second
            gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromMilliseconds(8); // clock ticks ever 120th a second
            gameTimer.Tick += GameLoop; // update the game state every 60th of a second
            gameTimer.Start();
        }


        private void GameLoop(object sender, EventArgs e)
        {
            UpdateGame();
            Render();
        }

        private void UpdateGame()
        { 
            // Check arrow key inputs to update paddle location
            if (Keyboard.IsKeyDown(Key.Left))
            {
                if (paddle.XCoords > 8)
                {
                    paddle.Move(-8);
                } else 
                {
                    paddle.XCoords = 0; // snap to game edge to avoid underdrawing

                }
            }
            if (Keyboard.IsKeyDown(Key.Right))
            {
                if (paddle.XCoords + paddleWidth < gameWidth - 8)
                {
                    paddle.Move(8);
                } else
                {
                    paddle.XCoords = gameWidth - paddleWidth; // snap to game edge to avoid overdrawing 
                }
            }
            // Check ball state
            if (!ball.Active)
            {
                ball.XPosition = paddle.XCoords + 37;
                if (Keyboard.IsKeyDown(Key.Space))
                {
                    ball.Active = true; // serve the ball if space is pressed
                    ball.YVelocity = -8;
                    ball.XVelocity = 8;
                }
            }
            // if ball is active run collision check
            if (ball.Active)
            {
                CalculateBallPosition();
            }
        }

        private void CalculateBallPosition()
        {
            // X-axis movement and collision
            float nextX = ball.XPosition + ball.XVelocity;
            if (nextX <= 0)
            {
                ball.XPosition = 0;
                ball.XVelocity = -ball.XVelocity;
            }
            else if (nextX + ballSize >= gameWidth)
            {
                ball.XPosition = gameWidth - ballSize;
                ball.XVelocity = -ball.XVelocity;
            }
            else
            {
                ball.XPosition = nextX;
            }

            // Y-axis movement and collision
            float nextY = ball.YPosition + ball.YVelocity;
            if (nextY <= 0)
            {
                ball.YPosition = 0;
                ball.YVelocity = -ball.YVelocity;
            }
            else if (nextY + ballSize >= gameHeight)
            {
                ball.YPosition = gameHeight - ballSize;
                ball.YVelocity = -ball.YVelocity;
            }
            else
            {
                ball.YPosition = nextY;
            }
        }




        // Redraw the game elements (ball, paddle) to the play field with updated coordinates
        private void Render()
        {
            Canvas.SetLeft(paddleImage, paddle.XCoords);
            Canvas.SetTop(paddleImage, paddle.YCoords);
            Canvas.SetLeft(ballImage, ball.XPosition);
            Canvas.SetTop(ballImage, ball.YPosition);
        }



    }
}