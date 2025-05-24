using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
/// <summary>
/// A classed used to represent graphical assets and  load them into the gam
/// </summary>
namespace Not_Breakout
{
    public static class Images
    {
        public readonly static ImageSource Paddle = LoadImage("paddle.png");
        public readonly static ImageSource Lava = LoadImage("lava-tile.png");
        public readonly static ImageSource Floor = LoadImage("floor-tile.png");
        public readonly static ImageSource Ball = LoadImage("ball.png");
        public readonly static ImageSource NormalBrick = LoadImage("basic-brick.png");
        public readonly static ImageSource HardBrick1 = LoadImage("hard-brick.png");
        public readonly static ImageSource HardBrick2 = LoadImage("hard-brick-2.png");
        public readonly static ImageSource HardBrick3 = LoadImage("hard-brick-3.png");

        private static ImageSource LoadImage(string fileName)
        {
            return new BitmapImage(new Uri($"Assets/{fileName}", UriKind.Relative));
        }
    }
}
