using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace selfdrivingcar.src.visual
{
    internal class VisualSegment
    {
        private readonly Segment _segment;
        private readonly Canvas _canvas;
        private Line? line;
        private static readonly SolidColorBrush DefaultStrokeColor = Brushes.Black;

        public VisualSegment(Segment segment, Canvas canvas)
        {
            _canvas = canvas;
            _segment = segment;
        }

        public void Draw(int width = 2, SolidColorBrush? color = null)
        {
            line = new Line()
            {
                StrokeThickness = width,
                Stroke = color ?? DefaultStrokeColor,
                X1 = _segment.PointA.coord.X,
                Y1 = _segment.PointA.coord.Y,
                X2 = _segment.PointB.coord.X,
                Y2 = _segment.PointB.coord.Y
            };

            AddToCanvas();
        }
        public void UnDraw() => RemoveFromCanvas();
        public void AddToCanvas() => _canvas.Children.Add(line);
        public void RemoveFromCanvas() => _canvas.Children.Remove(line);
    }
}
