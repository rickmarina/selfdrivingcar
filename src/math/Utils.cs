using selfdrivingcar.src.visual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
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

        public static Vector2 DirectionVector(Point pA, Point pB) {
            return Vector2.Normalize(Vector2.Subtract(pB.coord, pA.coord));
        } 
        public static float Distance(Point pA, Point pB)
        {
            return Vector2.Distance(pA.coord, pB.coord);
        }

        public static float DistanceFromPointToSegment(Point p, Segment s)
        {
            float dx = s.PointB.coord.X - s.PointA.coord.X;
            float dy = s.PointB.coord.Y - s.PointA.coord.Y;
            float longitudSegmentoCuadrado = dx * dx + dy * dy;
            if (longitudSegmentoCuadrado == 0)
                return Distance(p, s.PointA);

            // Proyección del punto P sobre el segmento
            float t = ((p.coord.X - s.PointA.coord.X) * dx + (p.coord.Y - s.PointA.coord.Y) * dy) / longitudSegmentoCuadrado;

            // Asegura que t esté en el rango [0, 1] para que caiga dentro del segmento
            t = Math.Max(0, Math.Min(1, t));

            // Encuentra el punto más cercano en el segmento a P
            Point nearPoint = new Point(s.PointA.coord.X + t * dx, s.PointA.coord.Y + t * dy);

            // Calcula la distancia entre P y el punto más cercano en el segmento
            return Distance(p, nearPoint);


        }
        public static Point Translate(Point loc, double angle, int offset)
        {
            return new Point((float)(loc.coord.X + Math.Cos(angle) * offset), (float)(loc.coord.Y + Math.Sin(angle) * offset));
        }

        public static double Angle(Point loc)
        {
            return Math.Atan2(loc.coord.Y, loc.coord.X);
        }

        public static (Point?, float) GetIntersection(Point A, Point B, Point C, Point D) {

            float tTop = (D.coord.X - C.coord.X) * (A.coord.Y - C.coord.Y) - (D.coord.Y - C.coord.Y) * (A.coord.X - C.coord.X);
            float uTop = (C.coord.Y - A.coord.Y) * (A.coord.X - B.coord.X) - (C.coord.X - A.coord.X) * (A.coord.Y - B.coord.Y);
            float bottom = (D.coord.Y - C.coord.Y) * (B.coord.X - A.coord.X) - (D.coord.X - C.coord.X) * (B.coord.Y - A.coord.Y);

            if (bottom != 0)
            {
                float t = tTop / bottom;
                float u = uTop / bottom;
                if (t >= 0 && t<= 1 && u >= 0 && u<= 1)
                {
                    return (new Point(Lerp(A.coord.X, B.coord.X, t), Lerp(A.coord.Y, B.coord.Y, t)), t);
                }
            }

            return (null, 0);
        }

        public static float Lerp(float a, float b, float t) => a+ (b-a)*t;

        public static Point Average(Point A, Point B)
        {
            return new Point((A.coord.X + B.coord.X) / 2, (A.coord.Y + B.coord.Y) / 2);
        }

        public static string GetObjectHash(object obj)
        {
            var options = new JsonSerializerOptions
            {
                IncludeFields = true  // Incluye los campos
            };
            var json = JsonSerializer.Serialize(obj, options);

            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(json);
                byte[] hashBytes = sha256.ComputeHash(bytes);

                var sb = new StringBuilder();
                foreach (var b in hashBytes)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
    }
}
