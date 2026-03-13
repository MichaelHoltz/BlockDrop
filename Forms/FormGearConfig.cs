using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BlockDrop.Classes.Gears;

namespace BlockDrop.Forms
{
    public partial class FormGearConfig : Form
    {
        private Gear insideGear;  // Lime green stationary gear
        private Gear outsideGear; // Light blue rotating gear
        private Timer animationTimer;
        private double rotationSpeed = 0.005; // Radians per tick
        private bool isPaused = false; // Track animation pause state
        private double orbitParameter = 0; // Tracks the orbit position

        public FormGearConfig()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            InitializeGears();
            SetupAnimation();
            
            // Add click event for pause/resume toggle
            this.MouseClick += FormGearConfig_MouseClick;
        }

        private void FormGearConfig_MouseClick(object sender, MouseEventArgs e)
        {
            // Toggle pause state
            isPaused = !isPaused;
            
            // Optional: Update form title to show paused state
            this.Text = isPaused ? "FormGearConfig - Paused" : "FormGearConfig";
        }

        private void InitializeGears()
        {
            // Calculate center of form
            PointF center = new PointF(this.ClientSize.Width / 2f, this.ClientSize.Height / 2f);
            
            // Use smallest dimension to ensure gear fits
            float formRadius = Math.Min(this.ClientSize.Width, this.ClientSize.Height) / 2f - 20;
            
            // Create inside gear (lime green, fills most of form)
            insideGear = new Gear
            {
                IsOutsideGear = false,
                Radius = formRadius,
                Position = center,
                GearColor = Color.LimeGreen,
                ShapeType = GearShapeType.Circle,
                TeethCount = 100,
                ToothPitch = 2.0
            };
            
            // Create outside gear (light blue, 20% of form radius)
            double outsideRadius = formRadius * 0.2;
            outsideGear = new Gear
            {
                IsOutsideGear = true,
                Radius = outsideRadius,
                Position = new PointF(center.X, center.Y - (float)(formRadius - outsideRadius)),
                GearColor = Color.LightBlue,
                ShapeType = GearShapeType.Circle,
                TeethCount = 20,
                ToothPitch = 2.0
            };
            
            // Reset orbit parameter when initializing
            orbitParameter = 0;
        }

        private void SetupAnimation()
        {
            animationTimer = new Timer();
            animationTimer.Interval = 16; // ~60 FPS
            animationTimer.Tick += AnimationTimer_Tick;
            animationTimer.Start();
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            // Skip animation update if paused
            if (isPaused)
                return;
            
            // Increment the orbit parameter
            orbitParameter += rotationSpeed;
            
            // Distance between centers (smooth gears touch at their radii)
            double centerDistance = insideGear.Radius - outsideGear.Radius;
            
            // Calculate new position along inside gear perimeter
            float newX = insideGear.Position.X + (float)(centerDistance * Math.Cos(orbitParameter));
            float newY = insideGear.Position.Y + (float)(centerDistance * Math.Sin(orbitParameter));
            
            outsideGear.Position = new PointF(newX, newY);
            
            // Calculate the actual rotation of the gear as it rolls inside
            // The gear rotates based on the arc length traveled divided by its radius
            // For rolling inside: rotation = -orbit * ((R - r) / r)
            outsideGear.RotationAngle = -orbitParameter * ((insideGear.Radius - outsideGear.Radius) / outsideGear.Radius);
            
            // Force redraw
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Draw inside gear (lime green) with 50% opacity
            Color insideColorTransparent = Color.FromArgb(128, insideGear.GearColor);
            Color insideBorderColor = Color.FromArgb(128, Color.DarkGreen);
            using (Brush insideBrush = new SolidBrush(insideColorTransparent))
            using (Pen insidePen = new Pen(insideBorderColor, 2))
            {
                float diameter = (float)insideGear.Radius * 2;
                g.FillEllipse(insideBrush, 
                    insideGear.Position.X - (float)insideGear.Radius, 
                    insideGear.Position.Y - (float)insideGear.Radius, 
                    diameter, diameter);
                g.DrawEllipse(insidePen, 
                    insideGear.Position.X - (float)insideGear.Radius, 
                    insideGear.Position.Y - (float)insideGear.Radius, 
                    diameter, diameter);
            }
            
            // Draw outside gear (light blue) with 50% opacity
            Color outsideColorTransparent = Color.FromArgb(128, outsideGear.GearColor);
            Color outsideBorderColor = Color.FromArgb(128, Color.DarkBlue);
            using (Brush outsideBrush = new SolidBrush(outsideColorTransparent))
            using (Pen outsidePen = new Pen(outsideBorderColor, 2))
            {
                float diameter = (float)outsideGear.Radius * 2;
                g.FillEllipse(outsideBrush, 
                    outsideGear.Position.X - (float)outsideGear.Radius, 
                    outsideGear.Position.Y - (float)outsideGear.Radius, 
                    diameter, diameter);
                g.DrawEllipse(outsidePen, 
                    outsideGear.Position.X - (float)outsideGear.Radius, 
                    outsideGear.Position.Y - (float)outsideGear.Radius, 
                    diameter, diameter);
                
                // Draw center dot to show rotation
                using (Brush centerBrush = new SolidBrush(outsideBorderColor))
                {
                    g.FillEllipse(centerBrush, 
                        outsideGear.Position.X - 3, 
                        outsideGear.Position.Y - 3, 
                        6, 6);
                }
                
                // Draw a line from center to edge to visualize rotation
                using (Pen rotationPen = new Pen(outsideBorderColor, 2))
                {
                    float lineX = outsideGear.Position.X + (float)(outsideGear.Radius * Math.Cos(outsideGear.RotationAngle));
                    float lineY = outsideGear.Position.Y + (float)(outsideGear.Radius * Math.Sin(outsideGear.RotationAngle));
                    g.DrawLine(rotationPen, outsideGear.Position, new PointF(lineX, lineY));
                }
            }
            
            // Draw pause indicator if paused
            if (isPaused)
            {
                using (Font font = new Font("Arial", 20, FontStyle.Bold))
                using (Brush textBrush = new SolidBrush(Color.FromArgb(180, 255, 255, 255)))
                {
                    string pauseText = "PAUSED";
                    SizeF textSize = g.MeasureString(pauseText, font);
                    g.DrawString(pauseText, font, textBrush, 
                        (this.ClientSize.Width - textSize.Width) / 2, 
                        20);
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (insideGear != null && outsideGear != null)
            {
                InitializeGears();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            animationTimer?.Stop();
            animationTimer?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
