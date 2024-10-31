using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace selfdrivingcar.src.visual
{
    internal class VisualPoint
    {
        private readonly Point _point;
        private readonly Canvas _canvas;
        private Ellipse? ellipse;
        private static readonly SolidColorBrush DefaultFillColor = Brushes.Black;
        private static readonly SolidColorBrush DefaultStrokeColor = Brushes.Black;

        public VisualPoint(Point point, Canvas canvas)
        {
            _point = point;
            _canvas = canvas;
        }

        public Point GetPoint() => _point;

        public void Draw(int size = 18, SolidColorBrush? color = null, int strokeThickness = 0, SolidColorBrush? strokeColor = null)
        {
            float rad = size / 2;

            ellipse = new Ellipse()
            {
                Fill = color ?? DefaultFillColor,
                Width = size,
                Height = size,
                RenderTransformOrigin = new System.Windows.Point(0.5, 0.5),
                StrokeThickness = 2,
                Stroke = strokeColor ?? DefaultStrokeColor
            };
            Canvas.SetLeft(ellipse, this._point.coord.X - rad);
            Canvas.SetTop(ellipse, this._point.coord.Y - rad);

            AddToCanvas();
        }

        public void DrawSelected()
        {
            Draw(strokeThickness: 2, strokeColor: Brushes.Yellow);
        }

        public void DrawHovered()
        {
            Draw(color: Brushes.Orange);
        }
        public void RestoreDefaultStyle()
        {
            if (ellipse is not null) {
                this.ellipse.Fill = DefaultFillColor;
                this.ellipse.Stroke = DefaultStrokeColor;
            }
        }
        public void UnDraw() => RemoveFromCanvas();
        public void Reset() { UnDraw(); Draw(); }

        public void AddToCanvas() => _canvas.Children.Add(ellipse);
        public void RemoveFromCanvas() => _canvas.Children.Remove(ellipse);
    }
}

