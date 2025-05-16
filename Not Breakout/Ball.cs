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
        public float XVelocity { get; set; }
        // y velocity of the ball
        public float YVelocity { get; set; }
        // x position of the ball in the playfield
        public float XPosition { get; set; }
        // y position of the ball in the playfield
        public float YPosition { get; set; }  
        // used to determine whether the ball is being served or is active
        public bool Active;

        // used to create a new ball instance
        internal Ball(float xVelocity, float yVelocity, int xPosition, int yPosition, bool active)
        {
            XVelocity = xVelocity;
            YVelocity = yVelocity;
            XPosition = xPosition;
            YPosition = yPosition;
            Active = active;
        }

        

    }
}
