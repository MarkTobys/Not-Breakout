using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// used to determine whether to change the balls X and Y velocity upon contact with the paddle
public enum Direction
{
    Left = -1,
    Right = 1,
    Still = 0
};

/// <summary>
/// Represents the paddle on the play field
/// Keeps track of the paddle's x and y coordinates, as well as the current movement status of the paddle
/// </summary>
namespace Not_Breakout
{
    public class Paddle
    {
        // current location of the paddle
        public int XCoords { get; set; }
        public int YCoords { get; set; }
        
        public Direction Moving { get; set; }

        // initialize a paddle instance with x and y coordinates
        public Paddle(int xCoords, int yCoords, Direction moving)
        {
            XCoords = xCoords;
            YCoords = yCoords;
            Moving = moving;
        }
    }
}
