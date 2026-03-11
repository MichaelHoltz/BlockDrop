using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockDrop
{
    public class BlockDropSettings: SettingsBase
    {
        public BlockDropSettings(string filePath) : base(filePath)
        {
        }

        /// <summary>
        /// Time in milliseconds for a block to travel exactly one row (also the spawn interval)
        /// </summary>
        [Category("Simulation")]
        [DisplayName("Row Travel Time (ms)")]
        [Description("Time in milliseconds for a block to travel exactly one row (also the spawn interval)")]
        public float RowTravelTimeMs { get; set; } = 250f;

        /// <summary>
        /// Direction of block movement.
        /// true  = blocks spawn at the top and fall down.
        /// false = blocks spawn at the bottom and rise up.
        /// </summary>
        [Category("Simulation")]
        [DisplayName("Move Down")]
        [Description("Direction of block movement. true = blocks spawn at the top and fall down. false = blocks spawn at the bottom and rise up.")]
        public bool MoveDown { get; set; } = true;

        [Category("Color")]
        [DisplayName("Use All Colors")]
        [Description("Whether to use all colors or just variations of the base color")]
        public bool UseAllColors { get; set; } = true;
        [Category("Color")]
        [DisplayName("Base Color")]
        [Description("Base color used for hue variations when Use All Colors is false")]
        public Color BaseColor { get; set; } = Color.FromArgb(0, 200, 0);
    }
}
