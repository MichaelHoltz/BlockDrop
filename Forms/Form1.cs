using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace BlockDrop.Forms
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// Number of columns in the grid (target ~50 blocks wide)
        /// </summary>
        private readonly int NumColumns = 75;

        /// <summary>
        /// Computed number of rows based on cell size and form height
        /// </summary>
        private int NumRows;

        /// <summary>
        /// Size in pixels of each square cell
        /// </summary>
        private int CellSize;

        /// <summary>
        /// Pixel offset to center the grid horizontally
        /// </summary>
        private int OffsetX;

        /// <summary>
        /// Pixel offset to center the grid vertically
        /// </summary>
        private int OffsetY;



        /// <summary>
        /// All blocks currently alive on the grid
        /// </summary>
        private List<Block> activeBlocks = new List<Block>();

        /// <summary>
        /// High-frequency timer that drives rendering at ~60 fps
        /// </summary>
        private Timer renderTimer;

        /// <summary>
        /// Stopwatch for accurate delta-time between frames
        /// </summary>
        private Stopwatch stopwatch = new Stopwatch();

        /// <summary>
        /// Accumulates elapsed time to decide when to spawn a new row
        /// </summary>
        private float spawnAccumulator = 0f;



        /// <summary>
        /// Base movement speed in rows per millisecond
        /// </summary>
        private float moveSpeed;

        /// <summary>
        /// Per-column speed multiplier (0.75 – 1.25) so each column drifts independently
        /// </summary>
        private float[] columnSpeedMultipliers;

        /// <summary>
        /// Shared random for spawn decisions
        /// </summary>
        private readonly Random random = new Random();

        public Form1()
        {
            InitializeComponent();

            this.Load += Form1_Load;
            this.Paint += Form1_Paint;
            this.FormClosing += Form1_FormClosing;
        }
        private PropertyForm _propertyForm;
        private BlockDropSettings _blockDropSettings;
        /// <summary>
        /// Initializes the grid after the form is fully laid out and maximized,
        /// so ClientSize reflects the actual screen dimensions.
        /// </summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            _blockDropSettings = BlockDropSettings.Load<BlockDropSettings>($@"C:\Projects_Sandbox\BlockDrop\settings.json");
            _propertyForm = new PropertyForm(_blockDropSettings, this);
            _propertyForm.TopMost = true;

            // Place PropertyForm on a different monitor if available
            Screen currentScreen = Screen.FromControl(this);
            Screen targetScreen = Screen.AllScreens
                .FirstOrDefault(s => !s.Equals(currentScreen)) ?? currentScreen;

            _propertyForm.StartPosition = FormStartPosition.Manual;
            _propertyForm.Location = new Point(
                targetScreen.WorkingArea.Left + (targetScreen.WorkingArea.Width - _propertyForm.Width) / 2,
                targetScreen.WorkingArea.Top + (targetScreen.WorkingArea.Height - _propertyForm.Height) / 2);

            _propertyForm.Show();

            // Now ClientSize is the real maximized size
            CellSize = ClientSize.Width / NumColumns;
            NumRows = ClientSize.Height / CellSize;

            // Center the grid within any leftover pixels
            OffsetX = (ClientSize.Width - (CellSize * NumColumns)) / 2;
            OffsetY = (ClientSize.Height - (CellSize * NumRows)) / 2;

            // 1 row per RowTravelTimeMs
            moveSpeed = 1.0f / _blockDropSettings.RowTravelTimeMs;

            // Assign each column a random speed multiplier between 0.75x and 1.25x
            columnSpeedMultipliers = new float[NumColumns];
            for (int col = 0; col < NumColumns; col++)
            {
                columnSpeedMultipliers[col] = 0.75f + (float)(random.NextDouble() * 0.5);
            }

            // Start simulation immediately at ~60 fps
            stopwatch.Start();
            renderTimer = new Timer();
            renderTimer.Interval = 16;
            renderTimer.Tick += RenderTimer_Tick;
            renderTimer.Start();
        }

        /// <summary>
        /// Called ~60 times per second. Advances blocks smoothly, spawns new rows
        /// on a fixed cadence, and repaints.
        /// </summary>
        private void RenderTimer_Tick(object sender, EventArgs e)
        {
            float deltaMs = (float)stopwatch.Elapsed.TotalMilliseconds;
            stopwatch.Restart();

            // Cap delta to avoid huge jumps (e.g. when the debugger pauses)
            if (deltaMs > 100f) deltaMs = 100f;

            UpdateBlocks(deltaMs);

            // Spawn on a fixed cadence independent of frame rate
            spawnAccumulator += deltaMs;
            while (spawnAccumulator >= _blockDropSettings.RowTravelTimeMs)
            {
                SpawnNewRow();
                spawnAccumulator -= _blockDropSettings.RowTravelTimeMs;
            }

            Invalidate();
        }

        /// <summary>
        /// Advances every block's smooth position, increments CurrentLifeMoves
        /// each time a full row boundary is crossed, computes fade opacity,
        /// and removes expired or off-screen blocks.
        /// </summary>
        private void UpdateBlocks(float deltaMs)
        {
            float direction = _blockDropSettings.MoveDown ? 1.0f : -1.0f;
            float fadeInRows = _blockDropSettings.FadeInRows;
            float fadeOutRows = _blockDropSettings.FadeOutRows;

            for (int i = activeBlocks.Count - 1; i >= 0; i--)
            {
                Block block = activeBlocks[i];
                float colMultiplier = columnSpeedMultipliers[block.AssignedColumn];
                block.RowPosition += moveSpeed * colMultiplier * deltaMs * direction;

                // How many full rows has this block travelled since spawn?
                float distance = _blockDropSettings.MoveDown
                    ? block.RowPosition
                    : (NumRows - 1) - block.RowPosition;

                int rowsCrossed = (int)distance;
                while (block.CurrentLifeMoves < rowsCrossed)
                    block.MoveBlock();

                // Compute fade-in / fade-out opacity
                float fadeIn = fadeInRows > 0f ? Math.Min(1f, distance / fadeInRows) : 1f;
                float remaining = block.MaxLifeMoves - distance;
                float fadeOut = fadeOutRows > 0f ? Math.Min(1f, remaining / fadeOutRows) : 1f;
                block.Opacity = Math.Max(0f, Math.Min(fadeIn, fadeOut));

                // Remove if expired or fully off-screen
                bool offScreen = _blockDropSettings.MoveDown
                    ? block.RowPosition >= NumRows
                    : block.RowPosition < -1f;

                if (block.IsExpired || offScreen)
                    activeBlocks.RemoveAt(i);
            }
        }

        /// <summary>
        /// Spawns a new row of blocks at the starting edge.
        /// Restriction 1 – at most 50 % of NumColumns are filled.
        /// Restriction 2 – a block is never placed directly adjacent to an
        ///                  existing block in the same column.
        /// </summary>
        private void SpawnNewRow()
        {
            int spawnRow = _blockDropSettings.MoveDown ? 0 : NumRows - 1;
            int adjacentRow = _blockDropSettings.MoveDown ? 1 : NumRows - 2;

            // Build set of columns that are blocked by nearby blocks
            HashSet<int> blockedColumns = new HashSet<int>();
            foreach (var block in activeBlocks)
            {
                int logicalRow = (int)Math.Floor(block.RowPosition);
                if (logicalRow == spawnRow || logicalRow == adjacentRow)
                    blockedColumns.Add(block.AssignedColumn);
            }

            // Collect eligible columns
            List<int> eligible = new List<int>();
            for (int col = 0; col < NumColumns; col++)
            {
                if (!blockedColumns.Contains(col))
                    eligible.Add(col);
            }

            // Restriction 1: never exceed 50 % of total columns
            int maxSpawn = NumColumns / 2;
            int count = random.Next(0, Math.Min(maxSpawn, eligible.Count) + 1);

            // Fisher-Yates shuffle then take the first 'count'
            for (int i = eligible.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                int tmp = eligible[i];
                eligible[i] = eligible[j];
                eligible[j] = tmp;
            }

            float startRow = _blockDropSettings.MoveDown ? 0f : NumRows - 1f;
            for (int i = 0; i < count; i++)
            {
                Block b = new Block(BackColor, NumRows, eligible[i],
                    _blockDropSettings.UseAllColors, _blockDropSettings.BaseColor);
                b.RowPosition = startRow;
                activeBlocks.Add(b);
            }
        }

        /// <summary>
        /// Draws every active block's alien glyph at its smooth floating-point row position
        /// with per-block opacity for fade-in / fade-out.
        /// </summary>
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            float penWidth = Math.Max(1f, CellSize * 0.08f);

            foreach (var block in activeBlocks)
            {
                if (block.Opacity <= 0f) continue;

                int alpha = (int)(block.Opacity * 255);
                Color fadedColor = Color.FromArgb(alpha, block.BlockColor);

                float x = OffsetX + block.AssignedColumn * CellSize;
                float y = OffsetY + block.RowPosition * CellSize;
                var cell = new RectangleF(x, y, CellSize - 1, CellSize - 1);

                AlienGlyphs.Draw(block.GlyphIndex, g, cell, fadedColor, penWidth);
            }
        }

        /// <summary>
        /// Cleans up the timer when the form closes.
        /// </summary>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            renderTimer.Stop();
            renderTimer.Dispose();
            stopwatch.Stop();
        }
    }
}
