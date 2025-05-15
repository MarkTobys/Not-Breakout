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
        // paddle used to deflect the ball
        private Paddle paddle;
        private Image paddleImage;
        // tick counter for the game
        private DispatcherTimer gameTimer;
        public MainWindow()
        {
            InitializeComponent();
            // Add the paddle image and draw it to the play field
            paddle = new Paddle(600, 600);
            paddleImage = new Image
            {
                Source = Images.Paddle,
                Width = 100,
                Height = 20
            };
            GameCanvas.Children.Add(paddleImage);
            // Listen for keypress events
            //this.KeyDown += OnKeyDown;
            // Set up game timer to run at 60 frames per second
            gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromMilliseconds(16); // ~60 FPS clock timer
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
            // Check ball collision 
            // Check key inputs to update paddle location
            if (Keyboard.IsKeyDown(Key.Left))
            {
                paddle.Move(-8);
            }
            if (Keyboard.IsKeyDown(Key.Right))
            {
                paddle.Move(8);
            }
        }

        // Redraw the game elements (ball, paddle) with updated coordinates
        private void Render()
        {
            Canvas.SetLeft(paddleImage, paddle.XCoords);
            Canvas.SetTop(paddleImage, paddle.YCoords);
        }



    }
}