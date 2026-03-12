using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlockDrop.Classes.BlockDrop
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
        /// Index into AlienGlyphs (0 – 49) identifying this block's symbol
        /// </summary>
        public int GlyphIndex { get; private set; }

        /// <summary>
        /// Current opacity (0.0 = invisible, 1.0 = fully opaque). Updated each frame by Form1.
        /// </summary>
        public float Opacity { get; set; }

        /// <summary>
        /// Whether this block has reached or exceeded its maximum life moves
        /// </summary>
        public bool IsExpired => CurrentLifeMoves >= MaxLifeMoves;

        /// <summary>
        /// Constructor for the block class. 
        /// </summary>
        /// <param name="backColor">Color to avoid</param>
        /// <param name="maxRows">Max Rows possible</param>
        /// <param name="assignedColumn">Column position</param>
        /// <param name="useAllColors">true = fully random colors, false = hue-locked to baseColor</param>
        /// <param name="baseColor">Base color whose hue is used when useAllColors is false</param>
        public Block(Color backColor, int maxRows, int assignedColumn, bool useAllColors = true, Color? baseColor = null)
        {
            BlockColor = useAllColors
                ? getRandomColor(backColor)
                : getRandomHueColor(baseColor ?? Color.Green, backColor);
            MaxLifeMoves = getMaxLifeMoves(maxRows);
            CurrentLifeMoves = 0;
            AssignedColumn = assignedColumn;
            GlyphIndex = rand.Next(AlienGlyphs.Count);
            Opacity = 0f;
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
        /// Generates a random color that shares the same hue as the base color
        /// but varies in saturation and brightness. Ensures contrast with the background.
        /// </summary>
        /// <param name="baseColor">Color whose hue to preserve</param>
        /// <param name="backColor">Background color to avoid</param>
        /// <returns>A random color with the same hue</returns>
        private Color getRandomHueColor(Color baseColor, Color backColor)
        {
            float hue = baseColor.GetHue();
            Color randomColor;
            do
            {
                // Saturation 0.4–1.0, Brightness 0.3–1.0 to keep vivid, visible results
                float saturation = 0.4f + (float)(rand.NextDouble() * 0.6);
                float brightness = 0.3f + (float)(rand.NextDouble() * 0.7);
                randomColor = ColorFromHSB(hue, saturation, brightness);
            } while (getColorDifference(randomColor, backColor) < 100);
            return randomColor;
        }

        /// <summary>
        /// Converts HSB (Hue 0–360, Saturation 0–1, Brightness 0–1) to a System.Drawing.Color.
        /// </summary>
        private static Color ColorFromHSB(float hue, float saturation, float brightness)
        {
            int hi = (int)(Math.Floor(hue / 60.0)) % 6;
            float f = (hue / 60.0f) - (float)Math.Floor(hue / 60.0);

            int v = (int)(brightness * 255);
            int p = (int)(brightness * (1 - saturation) * 255);
            int q = (int)(brightness * (1 - f * saturation) * 255);
            int t = (int)(brightness * (1 - (1 - f) * saturation) * 255);

            switch (hi)
            {
                case 0: return Color.FromArgb(v, t, p);
                case 1: return Color.FromArgb(q, v, p);
                case 2: return Color.FromArgb(p, v, t);
                case 3: return Color.FromArgb(p, q, v);
                case 4: return Color.FromArgb(t, p, v);
                default: return Color.FromArgb(v, p, q);
            }
        }

        /// <summary>
        /// Get the color difference between two colors. This is a simple sum of the absolute differences of the RGB components. A higher value means more different colors.
        /// </summary>
        private int getColorDifference(Color c1, Color c2)
        {
            return Math.Abs(c1.R - c2.R) + Math.Abs(c1.G - c2.G) + Math.Abs(c1.B - c2.B);
        }

        /// <summary>
        /// Function to get a random number that is at least 1/4 of the max rows and up to maxRows.
        /// </summary>
        private int getMaxLifeMoves(int maxRows)
        {
            return rand.Next(maxRows / 4, maxRows + 1);
        }
    }
}
