using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace selfdrivingcar.src
{
    internal class Graph
    {
        public List<Point> Points { get; set; }
        public List<Segment> Segments { get; set; }

        public Graph() { 
            Points = new List<Point>();
            Segments = new List<Segment>();
        }
        public Graph(List<Point> points, List<Segment> segments)
        {
            this.Points = points; 
            this.Segments= segments;
        }

        private void AddPoint(Point point) { this.Points.Add(point); }
        public bool TryAddPoint(Point point)
        {
            if (ContainsPoint(point) is null)
            {
                AddPoint(point);
                return true;
            }
            return false;
        }
        public Point? ContainsPoint(Point point) => this.Points.FirstOrDefault(p => p.Equals(point));

        public int RemovePoint(Point point)
        {
            int idx = this.Points.FindIndex(x => x.Equals(point));
            if (idx != -1)
            {
                this.Points.RemoveAt(idx);
            }
            return idx;
        }


        private void AddSegment(Segment segment) {this.Segments.Add(segment); }

        public bool TryAddSegment(Segment segment) {
            if (segment.PointA.Equals(segment.PointB))
                return false;

            if (ContainsSegment(segment) is null)
            {
                AddSegment(segment);
                return true; 
            }
            return false;
        }

        public Segment? ContainsSegment(Segment s) => this.Segments.FirstOrDefault(x => x.Equals(s));

        public void RemoveSegment(int index)
        {
            var segment = this.Segments[index];
            this.Segments.RemoveAt(index);
        }
        public int RemoveSegment(Segment seg)
        {
            int idx = this.Segments.FindIndex(x => x.Equals(seg));
            if (idx != -1)
            {
                this.Segments.RemoveAt(idx);
            }
            return idx;

        }

        public List<Segment> GetSegmentsWithPoint(Point point)
        {
            return Segments.Where(x => x.Includes(point)).ToList();
        }

        public void Dispose()
        {
            this.Points.Clear();
            this.Segments.Clear();
        }


    }
}
