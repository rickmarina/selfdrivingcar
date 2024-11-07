using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace selfdrivingcar.src.visual
{
    internal class VisualPolygon : VisualBase<Polygon>
    {
        private readonly List<Point> _points; 
        public VisualPolygon(Canvas canvas, List<Point> points) : base(canvas)
        {
            _points = points;
        }

        public void Draw()
        {
            shape = new Polygon()
            {
                Stroke = Brushes.Blue,
                StrokeThickness = 2,
                Fill = new SolidColorBrush(Color.FromArgb(30, 0, 0, 255)),

            };

            foreach(var p in _points) {
                shape.Points.Add(new System.Windows.Point(p.coord.X, p.coord.Y));
            }

            AddToCanvas();
        }

        public void UnDraw()
        {
            RemoveFromCanvas();
        }
    }
}
