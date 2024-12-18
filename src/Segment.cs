﻿using selfdrivingcar.src.math;
using System.Numerics;
using System.Windows.Media;
using System.Windows.Shapes;

namespace selfdrivingcar.src
{
    internal class Segment
    {
        public Point PointA { get; set; }
        public Point PointB { get; set; }

        public Segment(Point pointA, Point pointB) {
            PointA = pointA;
            PointB = pointB;
        }

        public float Length() => Utils.Distance(PointA, PointB);

        public Vector2 DirectionVector() => Utils.DirectionVector(PointA, PointB);

        public float DistanteToPoint(Point p) => Utils.DistanceFromPointToSegment(p, this);

        public override bool Equals(object? obj)
        {
            if (obj is Segment otherSegment)
            {
                return Includes(otherSegment.PointA) && Includes(otherSegment.PointB);
            }
            return false;
        }
        public override int GetHashCode()
        {
            // Ordena los puntos para que (PointA, PointB) y (PointB, PointA) tengan el mismo hash
            int hash1 = PointA.GetHashCode() ^ PointB.GetHashCode();
            int hash2 = PointB.GetHashCode() ^ PointA.GetHashCode();
            return hash1 ^ hash2;
        }

        public bool Includes(Point point)
        {
            return PointA.Equals(point) || PointB.Equals(point);
        }

        
    }
}
