using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace selfdrivingcar.src
{
    internal class PolygonG
    {
        public List<Point> _points { get; set; } 
        public PolygonG(List<Point> points)
        {
            _points = points;
        }
    }
}
