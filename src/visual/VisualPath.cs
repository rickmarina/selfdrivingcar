using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace selfdrivingcar.src.visual
{
    internal class VisualPath : VisualBase<Path>
    {
        private readonly List<Segment> _segments;
        public VisualPath(Canvas canvas, List<Segment> segments) : base(canvas)
        {
            _segments = segments;
        }

        public void Draw(SolidColorBrush strokeColor, double strokeThickness)
        {
            PathFigure figura = new PathFigure();
            figura.StartPoint = new System.Windows.Point(_segments[0].PointA.coord.X, _segments[0].PointA.coord.Y);
            figura.Segments.Add(new LineSegment(new System.Windows.Point(_segments[0].PointB.coord.X, _segments[0].PointB.coord.Y), true));
            Point lastPoint = _segments[0].PointB;

            for (int i = 1; i < _segments.Count; i++) {
                var current = _segments[i];
                if (lastPoint.Equals(current.PointA))
                {
                    figura.Segments.Add(new LineSegment(new System.Windows.Point(current.PointA.coord.X, current.PointA.coord.Y), true));
                    figura.Segments.Add(new LineSegment(new System.Windows.Point(current.PointB.coord.X, current.PointB.coord.Y), true));
                } else
                {
                    figura.Segments.Add(new LineSegment(new System.Windows.Point(current.PointA.coord.X, current.PointA.coord.Y), false));
                    figura.Segments.Add(new LineSegment(new System.Windows.Point(current.PointB.coord.X, current.PointB.coord.Y), true));
                }
                lastPoint = current.PointB;

            }

            // Crear la geometría de la figura
            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures.Add(figura);
            shape = new Path()
            {
                Stroke = strokeColor,
                StrokeThickness = strokeThickness,
                Data = pathGeometry
                
            };

            AddToCanvas(world.Enums.ZINDEXES.ROAD_LINES);
        }

        public void Undraw() => RemoveFromCanvas();
    }
}
