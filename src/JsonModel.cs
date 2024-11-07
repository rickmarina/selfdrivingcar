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
        public List<PointJson> Points { get; set; }
        public List<SegmentJson> Segments { get; set; }
    }
    public class PointJson
    {
        public CoordJson coord { get; set; }
    }
    public class CoordJson
    {
        public float X { get; set; }
        public float Y { get; set; }
    }

    public class SegmentJson
    {
        public PointAJson PointA { get; set; }
        public PointBJson PointB { get; set; }
    }
    public class PointAJson
    {
        public CoordJson coord { get; set; }
    }

    public class PointBJson
    {
        public CoordJson coord { get; set; }
    }
}
