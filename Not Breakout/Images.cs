using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Not_Breakout
{
    public static class Images
    {
        public readonly static ImageSource Paddle = LoadImage("paddle.png");
        public readonly static ImageSource Lava = LoadImage("lava-tile.png");
        public readonly static ImageSource Floor = LoadImage("floor-tile.png");
        public readonly static ImageSource Ball = LoadImage("ball.png");

        private static ImageSource LoadImage(string fileName)
        {
            return new BitmapImage(new Uri($"Assets/{fileName}", UriKind.Relative));
        }
    }
}
