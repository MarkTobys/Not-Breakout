using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// used to identify the type of brick and its qualities
public enum BrickType
{
    Empty,
    Normal,
    Hard1,
    Hard2,
    Hard3,
};

/// <summary>
/// Represents the array of bricks in the play field
/// Tracks the different types of bricks in each sector of the grid and the brick type for said sector, as well as the number of bricks left in the level
/// </summary>
namespace Not_Breakout
{
    public class BrickGrid
    {
        public BrickType[,] Grid { get; set; } // the grid array representing the bricks
        public int BricksLeft { get; set; } // the number of bricks left in the level
        
        public BrickGrid(int bricksLeft)
        {
            Grid = new BrickType[14, 10];
            BricksLeft = bricksLeft;
        }
    }
}
