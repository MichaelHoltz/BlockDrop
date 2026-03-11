using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlockDrop
{
    internal class Block
    {
        /// <summary>
        /// Shared random instance to avoid duplicate seeds when creating multiple blocks per tick
        /// </summary>
        private static readonly Random rand = new Random();

        /// <summary>
        /// Color assigned to this block
        /// </summary>
        public Color BlockColor { get; private set; }

        /// <summary>
        /// Max Life in moves for this block
        /// </summary>
        public int MaxLifeMoves { get; private set; }

        /// <summary>
        /// This is the Y position at the top of the screen or bottom depending on how the backdrop is setup. Both are options
        /// </summary>
        public int CurrentLifeMoves { get; private set; }

        /// <summary>
        /// The column position of this block
        /// </summary>
        public int AssignedColumn { get; private set; }

        /// <summary>
        /// Smooth row position used for rendering (fractional rows)
        /// </summary>
        public float RowPosition { get; set; }

        /// <summary>
        /// Whether this block has reached or exceeded its maximum life moves
        /// </summary>
        public bool IsExpired => CurrentLifeMoves >= MaxLifeMoves;

        /// <summary>
        /// Constructor for the block class. 
        /// </summary>
        /// <param name="backColor">Color to avoid</param>
        /// <param name="maxRows">Max Rows possible </param>
        /// <param name="assignedColumn">Max Columns possible </param>
        public Block(Color backColor, int maxRows, int assignedColumn)
        {
            BlockColor = getRandomColor(backColor);
            MaxLifeMoves = getMaxLifeMoves(maxRows);
            CurrentLifeMoves = 0;
            AssignedColumn = assignedColumn;
        }

        /// <summary>
        /// Increments the current life moves by one. Called each time the block moves one row.
        /// </summary>
        public void MoveBlock()
        {
            CurrentLifeMoves++;
        }

        /// <summary>
        /// Function to generate a random color for the block that is not the same as the background color by a good margin.
        /// </summary> <param name="backColor">Color to avoid</param>
        /// <returns>A random color</returns>     
        private Color getRandomColor(Color backColor)
        {
            Color randomColor;
            do
            {
                randomColor = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
            } while (getColorDifference(randomColor, backColor) < 100); // Ensure a good contrast
            return randomColor;
        }

        /// <summary>
        /// Get the color difference between two colors. This is a simple sum of the absolute differences of the RGB components. A higher value means more different colors.
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        private int getColorDifference(Color c1, Color c2)
        {
            return Math.Abs(c1.R - c2.R) + Math.Abs(c1.G - c2.G) + Math.Abs(c1.B - c2.B);
        }

        /// <summary>
        /// Function to get a random number that is at least 1/4 of the max rows and up to maxRows.
        /// </summary>
        /// <param name="maxRows"></param>
        /// <returns></returns>
        private int getMaxLifeMoves(int maxRows)
        {
            return rand.Next(maxRows / 4, maxRows + 1);
        }
    }
}
