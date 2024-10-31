using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace selfdrivingcar.src.visual
{
    internal class VisualSegment : VisualBase<Line> 
    {
        private readonly Segment _segment;
        private static readonly SolidColorBrush DefaultStrokeColor = Brushes.Black;

        public VisualSegment(Segment segment, Canvas canvas) : base(canvas)
        {
            _segment = segment;
        }

        public void Draw(int width = 2, SolidColorBrush? color = null)
        {
            shape = new Line()
            {
                StrokeThickness = width,
                Stroke = color ?? DefaultStrokeColor,
                X1 = _segment.PointA.coord.X,
                Y1 = _segment.PointA.coord.Y,
                X2 = _segment.PointB.coord.X,
                Y2 = _segment.PointB.coord.Y,
                StrokeDashArray = [4,2]
            };

            AddToCanvas();
        }
        public void UnDraw() => RemoveFromCanvas();
        
    }
}
