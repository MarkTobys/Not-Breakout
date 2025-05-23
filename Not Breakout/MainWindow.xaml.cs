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
            paddle = new Paddle(400, 600, Direction.Still);
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
            gameTimer.Interval = TimeSpan.FromMilliseconds(12); // clock ticks ever 90th a second
            gameTimer.Tick += GameLoop; // update the game state every 90th of a second
            gameTimer.Start();
        }

        /// <summary>
        ///  main game loop, subscribed to the gameTimer timer, which updates the state of the game and redraws it 
        /// </summary>
        private void GameLoop(object sender, EventArgs e)
        {
            UpdateGame(); 
            Render();
        }

        /// <summary>
        /// updates the state of the game to update the location and behaviour of the ball/paddle based on user input and collisions with walls and the paddle
        /// </summary>
        private void UpdateGame()
        { 
            // Check arrow key inputs to update paddle location and the paddles direction of travel
            if (Keyboard.IsKeyDown(Key.Left) && !Keyboard.IsKeyDown(Key.Right))
            {
                if (paddle.XCoords > 8)
                {
                    paddle.Move(-8);
                    paddle.Moving = Direction.Left;
                } else 
                {
                    paddle.XCoords = 0; // snap paddle to game edge to avoid underdrawing
                    paddle.Moving = Direction.Left;
                }
            }
            else if (Keyboard.IsKeyDown(Key.Right) && !Keyboard.IsKeyDown(Key.Left))
            {
                if (paddle.XCoords + paddleWidth < gameWidth - 8)
                {
                    paddle.Move(8);
                    paddle.Moving = Direction.Right;
                } else
                {
                    paddle.XCoords = gameWidth - paddleWidth; // snap paddle to game edge to avoid overdrawing 
                    paddle.Moving = Direction.Right;
                }
            } else
            {
                paddle.Moving = Direction.Still;
            }
            // check if ball is in service mode either move ball to track paddle or serve ball if space is held down
            if (!ball.Active)
            {
                ball.XPosition = paddle.XCoords + 37; // center of paddle (50px) - half of ball size (26px) to draw ball in center of paddle
                if (Keyboard.IsKeyDown(Key.Space))
                {
                    ball.Active = true; // serve the ball if space is pressed
                    if (Keyboard.IsKeyDown(Key.Left) && !Keyboard.IsKeyDown(Key.Right))
                    {
                        ball.XVelocity = -5;
                        ball.YVelocity = -3.5f;
                    }
                    else if (Keyboard.IsKeyDown(Key.Right) && !Keyboard.IsKeyDown(Key.Left))
                    {
                        ball.XVelocity = 5;
                        ball.YVelocity = -3.5f;
                    }
                    else
                    {
                        ball.XVelocity = 0;
                        ball.YVelocity = -6;
                    }
                }
            }
            // if the ball is active and at same Y Coordinates or lower as the paddle, determine if it has colided with the paddle
            if (ball.YPosition != 0 && ball.YPosition + ballSize >= paddle.YCoords)
            {
                CalculatePaddleCollision();
            }
            // calulate balls next position
            
            CalculateBallPosition();
            
        }

        /// <summary>
        /// Determines if the ball is in a suitable X, Y coordinate space to be deflected by the paddle. If the paddle is moving then the ball will retain its 
        /// X velocity and the Y velocity will be inversed. If the paddle is still a new X, Y velocity will be calculated dependent on how close to the center
        /// of the paddle the ball hits. A direct center hit will direct the ball striaght up with 0 X velocity, a far edge hit will direct the ball outward with 
        /// an X velocity of 6 and a y velocity of -2.5/2.5 
        /// </summary>
        private void CalculatePaddleCollision()
        {
            // variables used to calculate the deflection angle of the ball
            float paddleCenter = paddle.XCoords + (paddleWidth / 2f);
            float ballCenter = ball.XPosition + (ballSize / 2f);
            float distanceFromCenter = ballCenter - paddleCenter;
            float ballYmiddle = ball.YPosition + (ballSize / 2);
            // check ball's Y direction of travel
            if (ball.YVelocity > 0)
            {
                // compare ball's Y hitbox (lower half of ball) to paddle hitbox (top 3rd of paddle)
                if (ballYmiddle <= paddle.YCoords + (paddleHeight * 0.33))
                {
                    if (ballCenter >= paddle.XCoords && ballCenter <= paddle.XCoords + paddleWidth)
                    {
                        ball.YPosition = paddle.YCoords - ballSize; // snap the ball out of the paddle if it is inside it to avoid multiple collisions
                        if ((float)paddle.Moving / ball.XVelocity > 0) // if ball is traveling in the same direction as paddle simply invert Y velocity 
                        {
                            ball.YVelocity = -ball.YVelocity;
                        }
                        else
                        {
                            // normalise the deflection distance to a range of [-1, 1]
                            float normalized = distanceFromCenter / (paddleWidth / 2f);
                            normalized = Math.Clamp(normalized, -1f, 1f);
                            // adjust X and Y velocity based on hit location
                            ball.XVelocity = normalized * 8f; // ranges from -8 to +8
                            ball.YVelocity = -6f + (Math.Abs(normalized) * 3.5f); // ranges from -6 (center) to -2.5 (edges)
                        }
                    }
                }
            } 
            else if (ball.YVelocity < 0)
            {
                // check if middle of ball is in paddle hitbox and that top of ball is not below paddle to avoid ball snapping ball to paddle
                if (ballYmiddle >= paddle.YCoords + (paddleHeight*0.66) && ball.YPosition <= paddle.YCoords + paddleHeight)
                {
                    if (ballCenter >= paddle.XCoords && ballCenter <= paddle.XCoords + paddleWidth)
                    {
                        ball.YPosition = paddle.YCoords + paddleHeight; // snap the ball out of the paddle if it is inside it to avoid multiple collisions
                        ball.YVelocity = -ball.YVelocity; // collision location does not impact ball's x,y velocity below paddle
                    }
                }
            }
        }

        /// <summary>
        /// calculates the balls next position in game and identifies any collisions with walls, reversing the balls X and Y velocity if a wall collision is detected
        /// if a wall collision is detected in the next step of the game the ball will be snapped to the wall it will collide with to avoid phasing into the wall
        /// </summary>
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




        /// <summary>
        /// Redraws the game state based on the new game element locations calculated in UpdateGame
        /// </summary>
        private void Render()
        {
            Canvas.SetLeft(paddleImage, paddle.XCoords);
            Canvas.SetTop(paddleImage, paddle.YCoords);
            Canvas.SetLeft(ballImage, ball.XPosition);
            Canvas.SetTop(ballImage, ball.YPosition);
        }



    }
}