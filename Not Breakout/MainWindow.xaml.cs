﻿using System.CodeDom;
using System.Numerics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
        // dictionary to map brick types to their appropriate images
        private readonly Dictionary<BrickType, ImageSource> gridValToImage = new()
        {
            {BrickType.Normal, Images.NormalBrick },
            {BrickType.Hard1, Images.HardBrick1 },
            {BrickType.Hard2, Images.HardBrick2},
            {BrickType.Hard3, Images.HardBrick3 }
        };
        // paddle and ball objects/images
        private Paddle paddle;
        private Ball ball;
        private Image paddleImage;
        private Image ballImage;
        // brick grid
        private BrickGrid brickGrid;
        // tick counter for the game
        private DispatcherTimer gameTimer;
        // Play area (canvas) constants;
        private const int gameWidth = 840;
        private const int gameHeight = 700;
        // paddle size constants
        private const int paddleWidth = 100;
        private const int paddleHeight = 20;
        // ball constant (width and height are the same size)
        private const int ballSize = 26;
        private const int ballRadius = 13;
        // rows and colums defined
        private int rows = 10;
        private int cols = 14;
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
            // fill the grid with bricks (test fill)
            brickGrid = new BrickGrid(140);
            for (int i = 0; i < cols; i++)
            {
                for (int j = 2; j < rows; j++)
                {
                    brickGrid.Grid[i, j] = BrickType.Normal;
                }
            }
            DrawBricks();
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
                    paddle.XCoords = paddle.XCoords -8;
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
                    paddle.XCoords = paddle.XCoords + 8;
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
            if (ball.YVelocity != 0 && ball.YPosition + ballSize >= paddle.YCoords)
            {
                CalculatePaddleCollision();
            }
            if (ball.YPosition <= 300)
            {
                CheckBrickCollision();
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
            float ballCenter = ball.XPosition + ballRadius;
            float distanceFromCenter = ballCenter - paddleCenter;
            float ballYmiddle = ball.YPosition + ballRadius;
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
        /// Determines if on the next tick, the ball will collide with a brick in the play field based on its current x,y coordinates + x,y velocity, updating 
        /// the brick grid to remove/damage the brick the ball has collided with and change the ball's x,y velocity accordingly
        /// </summary>
        private void CheckBrickCollision()
        {
            // use the current x,y velocity to determine the collision type (default: draw coordinates, top left corner)
            float ballNextX = ball.XVelocity > 0 ? ball.XPosition + ball.XVelocity + ballSize : ball.XPosition + ball.XVelocity;
            float ballNextY = ball.YVelocity < 0 ? ball.YPosition + ball.YVelocity : ball.YPosition + ball.YVelocity + ballSize;
            // calulate the grid sector the ball will be in next update
            int ballCol = (int)ballNextX / 60; 
            int ballRow = (int)ballNextY / 30;
            // handle OOB
            if (ballRow > 9) { ballRow = 9; }
            if (ballCol > 13) { ballCol = 13; }
            BrickType hitBrick = brickGrid.Grid[ballCol, ballRow];
            if (hitBrick != BrickType.Empty)
            {
                // if brick is normal or hard brick 3, lower brick count
                if (hitBrick == BrickType.Normal || hitBrick == BrickType.Hard3)
                {
                    brickGrid.BricksLeft--;
                }
                BrickType newBrick = GetWeakerBrick(hitBrick);
                brickGrid.Grid[ballCol, ballRow] = newBrick;
                DrawBricks();
            }
            
        }

        /// <summary>
        /// On brick collision, returns the updated brick type to be drawn based on the brick type that was hit
        /// </summary>
        /// <param name="hitBrick"> The brick type which was hit by the ball</param>
        /// <returns> Corresponding brick type post collision </returns>
        BrickType GetWeakerBrick(BrickType hitBrick)
        {
            return hitBrick switch
            {
                BrickType.Hard1 => BrickType.Hard1,
                BrickType.Hard2 => BrickType.Hard3,
                BrickType.Hard3 => BrickType.Empty,
                BrickType.Normal => BrickType.Empty,
                _ => hitBrick
            };
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
            // draw the ball and paddle
            Canvas.SetLeft(paddleImage, paddle.XCoords);
            Canvas.SetTop(paddleImage, paddle.YCoords);
            Canvas.SetLeft(ballImage, ball.XPosition);
            Canvas.SetTop(ballImage, ball.YPosition);
        }

        ///<summary>
        /// Draws the current state of the brick array when called
        /// Runs through each index of the brick array and draws the brick type stored in each index to its corresponding location 
        /// in the brick grid on the play field. This function should only be called when a brick is broken/weakened to avoid 
        /// unnecessary redrawing of the brick grid
        /// </summary>
        private void DrawBricks()
        {
            // clear the brick grid to remove any bricks that have been destroyed
            for (int i = GameCanvas.Children.Count - 1; i >= 0; i--)
            {
                if (GameCanvas.Children[i] is Image img && (string)img.Tag == "brick")
                {
                    GameCanvas.Children.RemoveAt(i);
                }
            }

            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    BrickType brick = brickGrid.Grid[i, j];
                    // Skip empty bricks
                    if (brick == BrickType.Empty)
                        continue;
                    Image brickImage = new Image
                    {
                        Source = gridValToImage[brick],
                        Width = 60,
                        Height = 30,
                        Tag = "brick" // tag bricks so when canvas is updated paddle/ball aren't deleted
                    }; 
                    // Draw the brick to the screen
                    Canvas.SetLeft(brickImage, i * 60);
                    Canvas.SetTop(brickImage, j * 30); // Use 'j' for vertical positioning
                    GameCanvas.Children.Add(brickImage);
                }
            }
        }


    }
}