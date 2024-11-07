using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace selfdrivingcar.src.visual
{
    internal class VisualPoint : VisualBase<Ellipse>
    {
        private readonly Point _point;
        private float Size = 18;
        private static readonly SolidColorBrush DefaultFillColor = Brushes.Black;
        private static readonly SolidColorBrush DefaultStrokeColor = Brushes.Black;
        private static readonly SolidColorBrush DefaultStrokeColorSelected = Brushes.Yellow;
        private static readonly SolidColorBrush DefaultFillColorHovered = Brushes.OrangeRed;
        private bool selected = false;
        private bool hovered = false; 

        public VisualPoint(Point point, Canvas canvas, float size = 18) : base(canvas)
        {
            _point = point;
            Size = size;
        }

        public Point GetPoint() => _point;

        public void UpdatePosition(Point p)
        {
            _point.coord = p.coord;

            float rad = Size / 2;
            Canvas.SetLeft(shape, this._point.coord.X - rad);
            Canvas.SetTop(shape, this._point.coord.Y - rad);
        }
        public void Draw(SolidColorBrush? color = null, int strokeThickness = 0, SolidColorBrush? strokeColor = null)
        {
            float rad = Size / 2;

            shape = new Ellipse()
            {
                Fill = color ?? DefaultFillColor,
                Width = Size,
                Height = Size,
                RenderTransformOrigin = new System.Windows.Point(0.5, 0.5),
                StrokeThickness = 2,
                Stroke = strokeColor ?? DefaultStrokeColor
            };
            Canvas.SetLeft(shape, this._point.coord.X - rad);
            Canvas.SetTop(shape, this._point.coord.Y - rad);

            AddToCanvas();
        }

        public void Selected(bool active)
        {
            if (shape != null)
            {
                if (active) { 
                    selected = true; 
                    shape.StrokeThickness = 2;
                    shape.Stroke = DefaultStrokeColorSelected;
                } else
                {
                    selected = false;
                    shape.StrokeThickness = 2;
                    shape.Stroke = DefaultStrokeColor;

                }
            }
        }
       

        public void Hover(bool active)
        {
            if (shape is not null)
            {
                if (active)
                {
                    hovered = true;
                    shape.Fill = DefaultFillColorHovered;
                } else
                {
                    hovered = false; 
                    shape.Fill = DefaultFillColor;
                }

            }
        }
        public void RestoreDefaultStyle()
        {
            if (shape is not null) {
                this.shape.Fill = DefaultFillColor;
                this.shape.Stroke = DefaultStrokeColor;
            }
        }
        public void UnDraw() => RemoveFromCanvas();
        public void Reset() { UnDraw(); Draw(); }

    }
}

