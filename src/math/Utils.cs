using selfdrivingcar.src.visual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace selfdrivingcar.src.math
{
    internal class Utils
    {
        public static VisualPoint? GetNearestPoint(Point location, List<VisualPoint> points, float threshold = float.MaxValue)
        {
            float minDist = float.MaxValue;

            VisualPoint? nearest = null; 
            foreach (VisualPoint point in points)
            {
                var distance = Distance(point.GetPoint(), location);
                if (distance < minDist && distance < threshold)
                {
                    minDist = distance;
                    nearest = point; 
                }
            }
            return nearest;
        }

        public static float Distance(Point pA, Point pB)
        {
            return Vector2.Distance(pA.coord, pB.coord);
        }

        public static Point Translate(Point loc, double angle, int offset)
        {
            return new Point((float)(loc.coord.X + Math.Cos(angle) * offset), (float)(loc.coord.Y + Math.Sin(angle) * offset));
        }

        public static double Angle(Point loc)
        {
            return Math.Atan2(loc.coord.Y, loc.coord.X);
        }
    }
}
