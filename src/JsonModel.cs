using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace selfdrivingcar.src
{
    internal class RootJson
    {
        public GraphJson? _graph { get; set; }

    }
    public class GraphJson
    {
        public required List<PointJson> Points { get; set; }
        public required List<SegmentJson> Segments { get; set; }
    }
    public class PointJson
    {
        public required CoordJson coord { get; set; }
    }
    public class CoordJson
    {
        public float X { get; set; }
        public float Y { get; set; }
    }

    public class SegmentJson
    {
        public required PointAJson PointA { get; set; }
        public required PointBJson PointB { get; set; }
    }
    public class PointAJson
    {
        public required CoordJson coord { get; set; }
    }

    public class PointBJson
    {
        public required CoordJson coord { get; set; }
    }
}
