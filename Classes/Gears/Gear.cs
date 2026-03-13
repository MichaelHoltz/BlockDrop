using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockDrop.Classes.Gears
{
    public class Gear
    {
        // Basic gear properties
        public bool IsOutsideGear { get; set; } = true;
        public double Radius { get; set; }
        public double RadiusY { get; set; } // Secondary radius for oval shapes
        public Color GearColor { get; set; } // Color for rendering the gear
        public double Opacity { get; set; } = 0.2; // Opacity from 0.0 to 1.0 (default 50%)
        
        // Shape definition using vertices for custom/spline shapes
        // Used primarily for inside gears with custom curves
        public List<PointF> ShapeVertices { get; set; }
        
        // Position and rotation state
        public PointF Position { get; set; }
        public double RotationAngle { get; set; } // Current rotation in radians
        
        // Trace points for drawing spirograph patterns
        public List<TracePoint> TracePoints { get; set; }
        
        // Shape type for rendering
        public GearShapeType ShapeType { get; set; }
        
        public Gear()
        {
            ShapeVertices = new List<PointF>();
            TracePoints = new List<TracePoint>();
            Position = new PointF(0, 0);
            RotationAngle = 0;
            ShapeType = GearShapeType.Circle;
            GearColor = Color.Gray;
            RadiusY = 0; // Will default to Radius if not set (circle)
        }
        
        // Add helper property to get effective Y radius
        public double EffectiveRadiusY
        {
            get { return RadiusY > 0 ? RadiusY : Radius; }
        }
        
        // Helper method to get the gear color with applied opacity
        public Color GetColorWithOpacity()
        {
            int alpha = (int)(Opacity * 255);
            alpha = Math.Max(0, Math.Min(255, alpha)); // Clamp to 0-255
            return Color.FromArgb(alpha, GearColor);
        }
        
        // Helper method to get a darker border color with applied opacity
        public Color GetBorderColorWithOpacity()
        {
            int alpha = (int)(Opacity * 255);
            alpha = Math.Max(0, Math.Min(255, alpha)); // Clamp to 0-255
            
            // Make border darker
            int r = Math.Max(0, GearColor.R - 40);
            int g = Math.Max(0, GearColor.G - 40);
            int b = Math.Max(0, GearColor.B - 40);
            
            return Color.FromArgb(alpha, r, g, b);
        }
        
        // Add a vertex to define custom shape (for inside gears)
        public void AddVertex(PointF vertex)
        {
            ShapeVertices.Add(vertex);
        }
        
        // Remove a vertex at specified index
        public void RemoveVertex(int index)
        {
            if (index >= 0 && index < ShapeVertices.Count)
            {
                ShapeVertices.RemoveAt(index);
            }
        }
        
        // Add a trace point at specified radius, angle, and color
        public void AddTracePoint(double radius, double angle, Color color)
        {
            TracePoints.Add(new TracePoint 
            { 
                Radius = radius, 
                Angle = angle, 
                TraceColor = color 
            });
        }
        
        // Remove a trace point
        public void RemoveTracePoint(int index)
        {
            if (index >= 0 && index < TracePoints.Count)
            {
                TracePoints.RemoveAt(index);
            }
        }
        
        // Calculate point on gear perimeter at given angle
        public PointF GetPointAtAngle(double angle)
        {
            if (ShapeType == GearShapeType.Circle)
            {
                double x = Position.X + Radius * Math.Cos(angle);
                double y = Position.Y + Radius * Math.Sin(angle);
                return new PointF((float)x, (float)y);
            }
            else if (ShapeType == GearShapeType.Custom && ShapeVertices.Count > 0)
            {
                // Interpolate between vertices for custom shapes
                // This is a simplified approach - you may want spline interpolation
                return InterpolateCustomShape(angle);
            }
            
            return Position;
        }
        
        private PointF InterpolateCustomShape(double angle)
        {
            if (ShapeVertices.Count == 0)
                return Position;
            
            // Normalize angle to [0, 2π]
            double normalizedAngle = angle % (2 * Math.PI);
            if (normalizedAngle < 0) normalizedAngle += 2 * Math.PI;
            
            // Find the appropriate segment
            double segmentAngle = (2 * Math.PI) / ShapeVertices.Count;
            int index = (int)(normalizedAngle / segmentAngle);
            int nextIndex = (index + 1) % ShapeVertices.Count;
            
            // Linear interpolation between vertices
            double t = (normalizedAngle - index * segmentAngle) / segmentAngle;
            float x = ShapeVertices[index].X + (float)t * (ShapeVertices[nextIndex].X - ShapeVertices[index].X);
            float y = ShapeVertices[index].Y + (float)t * (ShapeVertices[nextIndex].Y - ShapeVertices[index].Y);
            
            return new PointF(Position.X + x, Position.Y + y);
        }
    }
    
    public class TracePoint
    {
        public double Radius { get; set; } // Offset from gear center
        public double Angle { get; set; } // Angle relative to gear center
        public Color TraceColor { get; set; }
        public List<PointF> TracePath { get; set; } // Accumulated path points
        public bool IsEnabled { get; set; }
        
        public TracePoint()
        {
            TracePath = new List<PointF>();
            IsEnabled = true;
            TraceColor = Color.Black;
        }
        
        // Calculate absolute position based on gear state
        public PointF GetAbsolutePosition(Gear gear)
        {
            double totalAngle = gear.RotationAngle + Angle;
            double x = gear.Position.X + Radius * Math.Cos(totalAngle);
            double y = gear.Position.Y + Radius * Math.Sin(totalAngle);
            return new PointF((float)x, (float)y);
        }
    }
    
    public enum GearShapeType
    {
        Circle,
        Oval,
        Custom // Defined by ShapeVertices
    }
}
