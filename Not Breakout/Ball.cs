using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Not_Breakout
{
    public class Ball
    {
        // x velocity of the ball
        public float XVelocity { get; }
        // y velocity of the ball
        public float YVelocity { get; set; }
        // x position of the ball in the playfield
        public int XPosition;
        // y position of the ball in the playfield
        public int YPosition;

        // used to create a new ball instance
        internal Ball(float xVelocity, float yVelocity, int xPosition, int yPosition)
        {
            XVelocity = xVelocity;
            YVelocity = yVelocity;
            XPosition = xPosition;
            YPosition = yPosition;
        }
    }
}
