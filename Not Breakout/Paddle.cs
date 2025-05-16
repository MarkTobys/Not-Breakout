using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Not_Breakout
{
    public class Paddle
    {
        // current location of the paddle
        public int XCoords { get; set; }
        public int YCoords { get; set; }

        // initialize a paddle instance with x and y coordinates
        public Paddle(int xCoords, int yCoords)
        {
            XCoords = xCoords;
            YCoords = yCoords;
        }

        // Move the paddle on the x axis a set amount
        public void Move(int xUnits)
        {
            XCoords += xUnits;
        }

    }
}
