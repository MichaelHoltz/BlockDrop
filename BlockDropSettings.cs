using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockDrop
{
    public class BlockDropSettings
    {
        /// <summary>
        /// Time in milliseconds for a block to travel exactly one row (also the spawn interval)
        /// </summary>
        public float RowTravelTimeMs { get; set; } = 250f;
        public bool UseAllColors { get; set; } = true;
        public Color BaseColor { get; set; } = Color.FromArgb(0, 200, 0);
    }
}
