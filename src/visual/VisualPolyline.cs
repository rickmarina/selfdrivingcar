using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using selfdrivingcar.src.world;

namespace selfdrivingcar.src.visual
{
    internal class VisualPolyline : VisualBase<Polyline>
    {
        public List<Point> _points { get; private set; }
        public VisualPolyline(Canvas canvas, List<Point> points) : base(canvas)
        {
            _points = points;
        }

        public void UpdatePoly(List<Point> points)
        {
            this._points = points;
        }
        public void Draw()
        {
            shape = new Polyline()
            {
                Stroke = Brushes.Red,
                StrokeThickness = 2               

            };

            foreach (var p in _points)
            {
                shape.Points.Add(new System.Windows.Point(p.coord.X, p.coord.Y));
            }

            AddToCanvas(Enums.ZINDEXES.ROAD_LINES);
        }

        public void UnDraw()
        {
            RemoveFromCanvas();
        }
    }
}
