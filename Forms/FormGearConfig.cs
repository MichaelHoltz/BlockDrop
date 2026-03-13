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
        private double rotationSpeed = 0.15; // Radians per tick
        private bool isPaused = false; // Track animation pause state
        private double orbitParameter = 0; // Tracks the orbit position
        private List<PointF> tracePath; // Stores the spirograph trace
        private double traceRadiusPercent = 0.844; // 90% of gear radius
        private double accumulatedArcLength = 0; // Track total arc length for rotation calculation

        public FormGearConfig()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            tracePath = new List<PointF>();
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
            };
            
            // Create outside gear (light blue, oval shape)
            double outsideRadius = formRadius * 0.5;
            outsideGear = new Gear
            {
                IsOutsideGear = true,
                Radius = outsideRadius,
                RadiusY = outsideRadius * 0.9, // Make it 60% height for oval
                Position = new PointF(center.X, center.Y - (float)(formRadius - outsideRadius)),
                GearColor = Color.LightBlue,
                ShapeType = GearShapeType.Oval,
                RotationAngle = 0
            };
            
            // Reset orbit parameter, rotation, arc length, and clear trace when initializing
            orbitParameter = 0;
            accumulatedArcLength = 0;
            tracePath.Clear();
        }

        private void SetupAnimation()
        {
            animationTimer = new Timer();
            animationTimer.Interval = 16; // ~60 FPS
            animationTimer.Tick += AnimationTimer_Tick;
            animationTimer.Start();
        }

        // Calculate the radius of the gear at a given angle (for ovals/ellipses)
        private double GetRadiusAtAngle(Gear gear, double angle)
        {
            if (gear.ShapeType == GearShapeType.Circle)
            {
                return gear.Radius;
            }
            else if (gear.ShapeType == GearShapeType.Oval)
            {
                // Ellipse formula: r(θ) = (a*b) / sqrt((b*cos(θ))^2 + (a*sin(θ))^2)
                double a = gear.Radius;
                double b = gear.EffectiveRadiusY;
                double cosTheta = Math.Cos(angle);
                double sinTheta = Math.Sin(angle);
                return (a * b) / Math.Sqrt((b * cosTheta) * (b * cosTheta) + (a * sinTheta) * (a * sinTheta));
            }
            return gear.Radius;
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            // Skip animation update if paused
            if (isPaused)
                return;
            
            // Store previous orbit parameter
            double prevOrbitParameter = orbitParameter;
            
            // Increment the orbit parameter
            orbitParameter += rotationSpeed;
            double dTheta = orbitParameter - prevOrbitParameter;
            
            // For current rotation angle, find contact angle and radius
            double contactAngle = orbitParameter + Math.PI - outsideGear.RotationAngle;
            
            // Normalize angle to [0, 2π]
            contactAngle = contactAngle % (2 * Math.PI);
            if (contactAngle < 0) contactAngle += 2 * Math.PI;
            
            // Get radius at contact point
            double rContact = GetRadiusAtAngle(outsideGear, contactAngle);
            
            // Distance between centers
            double centerDistance = insideGear.Radius - rContact;
            
            // Arc length traveled by the gear center along its orbit
            double arcLength = centerDistance * dTheta;
            accumulatedArcLength += arcLength;
            
            // The gear rotates based on accumulated arc length divided by current contact radius
            // For rolling inside: rotation is negative (opposite direction)
            outsideGear.RotationAngle = -accumulatedArcLength / GetAverageRadius(outsideGear);
            
            // Recalculate contact angle and distance with updated rotation
            contactAngle = orbitParameter + Math.PI - outsideGear.RotationAngle;
            contactAngle = contactAngle % (2 * Math.PI);
            if (contactAngle < 0) contactAngle += 2 * Math.PI;
            
            rContact = GetRadiusAtAngle(outsideGear, contactAngle);
            centerDistance = insideGear.Radius - rContact;
            
            // Calculate new position along inside gear perimeter
            float newX = insideGear.Position.X + (float)(centerDistance * Math.Cos(orbitParameter));
            float newY = insideGear.Position.Y + (float)(centerDistance * Math.Sin(orbitParameter));
            
            outsideGear.Position = new PointF(newX, newY);
            
            // Calculate trace point position (at traceRadiusPercent of gear radius along the major axis)
            double traceRadius = outsideGear.Radius * traceRadiusPercent;
            float traceX = outsideGear.Position.X + (float)(traceRadius * Math.Cos(outsideGear.RotationAngle));
            float traceY = outsideGear.Position.Y + (float)(traceRadius * Math.Sin(outsideGear.RotationAngle));
            
            // Add to trace path
            tracePath.Add(new PointF(traceX, traceY));
            
            // Limit trace path length to prevent memory issues (keep last 10000 points)
            if (tracePath.Count > 10000)
            {
                tracePath.RemoveAt(0);
            }
            
            // Force redraw
            this.Invalidate();
        }

        // Get average radius for the gear (used for rotation calculation)
        private double GetAverageRadius(Gear gear)
        {
            if (gear.ShapeType == GearShapeType.Circle)
            {
                return gear.Radius;
            }
            else if (gear.ShapeType == GearShapeType.Oval)
            {
                // For ellipse, use the geometric mean of the two radii
                return Math.Sqrt(gear.Radius * gear.EffectiveRadiusY);
            }
            return gear.Radius;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Draw trace path first (so it appears behind gears)
            if (tracePath.Count > 1)
            {
                using (Pen tracePen = new Pen(Color.Red, 2))
                {
                    g.DrawLines(tracePen, tracePath.ToArray());
                }
            }
            
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
            
            // Draw outside gear (light blue) with 50% opacity - support oval
            Color outsideColorTransparent = Color.FromArgb(128, outsideGear.GearColor);
            Color outsideBorderColor = Color.FromArgb(128, Color.DarkBlue);
            using (Brush outsideBrush = new SolidBrush(outsideColorTransparent))
            using (Pen outsidePen = new Pen(outsideBorderColor, 2))
            {
                float width = (float)outsideGear.Radius * 2;
                float height = (float)outsideGear.EffectiveRadiusY * 2;
                
                // Save graphics state for rotation
                GraphicsState state = g.Save();
                
                // Rotate around the gear center to orient the oval
                g.TranslateTransform(outsideGear.Position.X, outsideGear.Position.Y);
                g.RotateTransform((float)(outsideGear.RotationAngle * 180 / Math.PI));
                
                // Draw oval centered at origin
                g.FillEllipse(outsideBrush, -width / 2, -height / 2, width, height);
                g.DrawEllipse(outsidePen, -width / 2, -height / 2, width, height);
                
                // Draw center dot
                using (Brush centerBrush = new SolidBrush(outsideBorderColor))
                {
                    g.FillEllipse(centerBrush, -3, -3, 6, 6);
                }
                
                // Draw line to show rotation (pointing to the right in local coordinates)
                using (Pen rotationPen = new Pen(outsideBorderColor, 2))
                {
                    g.DrawLine(rotationPen, 0, 0, width / 2, 0);
                }
                
                // Restore graphics state
                g.Restore(state);
            }
            
            // Draw trace point indicator
            if (tracePath.Count > 0)
            {
                PointF currentTracePoint = tracePath[tracePath.Count - 1];
                using (Brush tracePointBrush = new SolidBrush(Color.Red))
                {
                    g.FillEllipse(tracePointBrush, 
                        currentTracePoint.X - 4, 
                        currentTracePoint.Y - 4, 
                        8, 8);
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
