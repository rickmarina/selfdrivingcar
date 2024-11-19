using selfdrivingcar.src.math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace selfdrivingcar.src
{
    internal class PolygonG
    {
        public List<Point> Points { get; set; }
        public List<Segment> Segments { get; private set; }

        public PolygonG(List<Point> points)
        {
            Points = points;

            Segments = new();
            for (int i= 1; i<= points.Count; i++)
            {
                Segments.Add(new Segment(points[i - 1], points[i % points.Count]));
            }

        }

        public static List<Segment> Union(List<PolygonG> polygons)
        {
            MultiBreak(polygons);
            List<Segment> keptSegments = new(); 

            for (int i=0; i< polygons.Count; i++)
            {
                foreach (var seg in polygons[i].Segments)
                {
                    bool keep = true; 
                    for (int j =0; j < polygons.Count;j++)
                    {
                        if (i!= j)
                        {
                            if (polygons[j].ContainsSegment(seg))
                            {
                                keep = false; 
                                break;
                            }
                        }
                    }
                    if (keep) 
                        keptSegments.Add(seg);
                }
            }

            return keptSegments;
        }

        public bool ContainsSegment(Segment seg) {  
            var midPoint = Utils.Average(seg.PointA, seg.PointB);
            return ContainsPoint(midPoint);
        }

        public bool ContainsPoint(Point point)
        {
            Point outerPoint = new Point(-1000, -1000);
            int intersectionCount = 0; 
            foreach (var seg in Segments)
            {
                (var intersection, var offset) = Utils.GetIntersection(outerPoint, point, seg.PointA, seg.PointB);
                if (intersection != null)
                {
                    intersectionCount++;
                }
            }

            return (intersectionCount & 1) == 1; //odd contains the point

        }

        public float DistanceToPoint(Point point)
        {
            return Segments.Select(x => x.DistanteToPoint(point)).Min();
        }

        public float DistanceToPoly(PolygonG poly)
        {
            return Points.Select(x => poly.DistanceToPoint(x)).Min();
        }

        public bool IntersectsPoly(PolygonG poly)
        {
            foreach (var s1 in Segments)
            {
                foreach (var s2 in poly.Segments)
                {
                    (var intersectionPoint, float offset) = (Utils.GetIntersection(s1.PointA, s1.PointB, s2.PointA, s2.PointB));
                    if (intersectionPoint != null)
                        return true;
                }
            }
            return false;
        }

        public static void MultiBreak(List<PolygonG> polys)
        {
            for (int i = 0; i < polys.Count-1; i++)
            {
                for (int j = i+1; j < polys.Count ; j++)
                {
                    PolygonG.Break(polys[i], polys[j]);
                }


            }
        }

        public static void Break(PolygonG poly1, PolygonG poly2)
        {
            var segs1 = poly1.Segments;
            var segs2 = poly2.Segments;

            for (int i = 0; i < segs1.Count;i++)
            {
                for (int j = 0; j < segs2.Count;j++)
                {
                    (var intersection, float offset) = Utils.GetIntersection(segs1[i].PointA, segs1[i].PointB, segs2[j].PointA, segs2[j].PointB);
                    if (intersection != null && offset != 0 && offset != 1) { 
                        var point = new Point(intersection.coord.X, intersection.coord.Y);
                        var aux = segs1[i].PointB;
                        segs1[i].PointB = point;
                        segs1.Insert(i + 1, new Segment(point, aux));

                        aux = segs2[j].PointB;
                        segs2[j].PointB = point;
                        segs2.Insert(j + 1, new Segment(point, aux));


                    }
                }
            }



        }
    }
}
