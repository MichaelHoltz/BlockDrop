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
        private double rotationSpeed = 0.05; // Radians per tick
        private bool isPaused = false; // Track animation pause state
        private double insideToothHeight;
        private double outsideToothHeight;

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
            
            // Calculate tooth heights for both gears
            double insideCircumference = 2 * Math.PI * insideGear.Radius;
            double insidePitchPerTooth = insideCircumference / insideGear.TeethCount;
            insideToothHeight = insidePitchPerTooth * 0.5;
            
            // Create outside gear (light blue, 20% of form radius)
            double outsideRadius = formRadius * 0.2;
            outsideGear = new Gear
            {
                IsOutsideGear = true,
                Radius = outsideRadius,
                Position = center,
                GearColor = Color.LightBlue,
                ShapeType = GearShapeType.Circle,
                TeethCount = 20,
                ToothPitch = 2.0
            };
            
            double outsideCircumference = 2 * Math.PI * outsideGear.Radius;
            double outsidePitchPerTooth = outsideCircumference / outsideGear.TeethCount;
            outsideToothHeight = outsidePitchPerTooth * 0.5;
            
            // Position the outside gear accounting for tooth heights
            double centerDistance = insideGear.Radius - insideToothHeight - (outsideGear.Radius + outsideToothHeight);
            outsideGear.Position = new PointF(center.X, center.Y - (float)centerDistance);
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
            
            // Rotate the outside gear clockwise
            outsideGear.RotationAngle += rotationSpeed;
            
            // Calculate orbit angle for the outside gear's position
            double orbitAngle = outsideGear.RotationAngle * (insideGear.Radius / outsideGear.Radius);
            
            // Distance between centers accounting for tooth heights
            double centerDistance = insideGear.Radius - insideToothHeight - (outsideGear.Radius + outsideToothHeight);
            
            // Calculate new position along inside gear perimeter
            float newX = insideGear.Position.X + (float)(centerDistance * Math.Cos(orbitAngle));
            float newY = insideGear.Position.Y + (float)(centerDistance * Math.Sin(orbitAngle));
            
            outsideGear.Position = new PointF(newX, newY);
            
            // Force redraw
            this.Invalidate();
        }

        private void DrawGearWithTeeth(Graphics g, Gear gear, Color fillColor, Color borderColor)
        {
            // Calculate tooth dimensions based on circumference and tooth count
            double circumference = 2 * Math.PI * gear.Radius;
            double pitchPerTooth = circumference / gear.TeethCount;
            
            // Tooth height is a standard fraction of the pitch
            double toothHeight = pitchPerTooth * 0.5;
            
            // Create graphics path for gear with teeth
            using (GraphicsPath gearPath = new GraphicsPath())
            {
                double anglePerTooth = (2 * Math.PI) / gear.TeethCount;
                double toothWidthAngle = anglePerTooth * 0.45; // 45% of the angle for tooth
                
                List<PointF> points = new List<PointF>();
                
                for (int i = 0; i < gear.TeethCount; i++)
                {
                    // Base angle for this tooth, adjusted by gear rotation
                    double baseAngle = i * anglePerTooth + gear.RotationAngle;
                    
                    if (gear.IsOutsideGear)
                    {
                        // Outside gear - teeth point outward
                        double innerRadius = gear.Radius;
                        double outerRadius = gear.Radius + toothHeight;
                        
                        // Left side of tooth valley
                        double angle1 = baseAngle - anglePerTooth / 2;
                        points.Add(new PointF(
                            gear.Position.X + (float)(innerRadius * Math.Cos(angle1)),
                            gear.Position.Y + (float)(innerRadius * Math.Sin(angle1))));
                        
                        // Transition to tooth
                        double angle2 = baseAngle - toothWidthAngle / 2;
                        points.Add(new PointF(
                            gear.Position.X + (float)(innerRadius * Math.Cos(angle2)),
                            gear.Position.Y + (float)(innerRadius * Math.Sin(angle2))));
                        
                        // Left side of tooth tip
                        points.Add(new PointF(
                            gear.Position.X + (float)(outerRadius * Math.Cos(angle2)),
                            gear.Position.Y + (float)(outerRadius * Math.Sin(angle2))));
                        
                        // Right side of tooth tip
                        double angle3 = baseAngle + toothWidthAngle / 2;
                        points.Add(new PointF(
                            gear.Position.X + (float)(outerRadius * Math.Cos(angle3)),
                            gear.Position.Y + (float)(outerRadius * Math.Sin(angle3))));
                        
                        // Transition from tooth
                        points.Add(new PointF(
                            gear.Position.X + (float)(innerRadius * Math.Cos(angle3)),
                            gear.Position.Y + (float)(innerRadius * Math.Sin(angle3))));
                    }
                    else
                    {
                        // Inside gear - teeth point inward
                        double outerRadius = gear.Radius;
                        double innerRadius = gear.Radius - toothHeight;
                        
                        // Left side of tooth valley
                        double angle1 = baseAngle - anglePerTooth / 2;
                        points.Add(new PointF(
                            gear.Position.X + (float)(outerRadius * Math.Cos(angle1)),
                            gear.Position.Y + (float)(outerRadius * Math.Sin(angle1))));
                        
                        // Transition to tooth
                        double angle2 = baseAngle - toothWidthAngle / 2;
                        points.Add(new PointF(
                            gear.Position.X + (float)(outerRadius * Math.Cos(angle2)),
                            gear.Position.Y + (float)(outerRadius * Math.Sin(angle2))));
                        
                        // Left side of tooth tip
                        points.Add(new PointF(
                            gear.Position.X + (float)(innerRadius * Math.Cos(angle2)),
                            gear.Position.Y + (float)(innerRadius * Math.Sin(angle2))));
                        
                        // Right side of tooth tip
                        double angle3 = baseAngle + toothWidthAngle / 2;
                        points.Add(new PointF(
                            gear.Position.X + (float)(innerRadius * Math.Cos(angle3)),
                            gear.Position.Y + (float)(innerRadius * Math.Sin(angle3))));
                        
                        // Transition from tooth
                        points.Add(new PointF(
                            gear.Position.X + (float)(outerRadius * Math.Cos(angle3)),
                            gear.Position.Y + (float)(outerRadius * Math.Sin(angle3))));
                    }
                }
                
                // Close the path
                if (points.Count > 0)
                {
                    gearPath.AddPolygon(points.ToArray());
                }
                
                // Fill and draw the gear
                using (Brush brush = new SolidBrush(fillColor))
                using (Pen pen = new Pen(borderColor, 1))
                {
                    g.FillPath(brush, gearPath);
                    g.DrawPath(pen, gearPath);
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Draw inside gear (lime green) with 50% opacity and teeth
            Color insideColorTransparent = Color.FromArgb(128, insideGear.GearColor);
            Color insideBorderColor = Color.FromArgb(128, Color.DarkGreen);
            DrawGearWithTeeth(g, insideGear, insideColorTransparent, insideBorderColor);
            
            // Draw outside gear (light blue) with 50% opacity and teeth
            Color outsideColorTransparent = Color.FromArgb(128, outsideGear.GearColor);
            Color outsideBorderColor = Color.FromArgb(128, Color.DarkBlue);
            DrawGearWithTeeth(g, outsideGear, outsideColorTransparent, outsideBorderColor);
            
            // Draw center dot to show rotation
            using (Brush centerBrush = new SolidBrush(outsideBorderColor))
            {
                g.FillEllipse(centerBrush, 
                    outsideGear.Position.X - 3, 
                    outsideGear.Position.Y - 3, 
                    6, 6);
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
