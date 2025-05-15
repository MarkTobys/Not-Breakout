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
        public double XCoords { get; set; }
        public double YCoords { get; set; }

        // initialize a paddle instance with x and y coordinates
        public Paddle(double xCoords, double yCoords)
        {
            XCoords = xCoords;
            YCoords = yCoords;
        }

        // Move the paddle on the x axis a set amount
        public void Move(double xUnits)
        {
            XCoords += xUnits;
        }

    }
}
