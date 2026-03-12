using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockDrop.Settings
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

        /// <summary>
        /// Number of rows over which a new block fades from invisible to fully opaque.
        /// Set to 0 for instant appearance.
        /// </summary>
        [Category("Simulation")]
        [DisplayName("Fade-In Rows")]
        [Description("Number of rows a block travels while fading in. 0 = instant.")]
        public float FadeInRows { get; set; } = 3f;

        /// <summary>
        /// Number of rows before expiry over which a block fades from fully opaque to invisible.
        /// Set to 0 for instant disappearance.
        /// </summary>
        [Category("Simulation")]
        [DisplayName("Fade-Out Rows")]
        [Description("Number of rows before expiry a block spends fading out. 0 = instant.")]
        public float FadeOutRows { get; set; } = 3f;

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
