﻿using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace selfdrivingcar.src.visual
{
    internal class VisualPoint : VisualBase<Ellipse>
    {
        private readonly Point _point;
        private static readonly SolidColorBrush DefaultFillColor = Brushes.Black;
        private static readonly SolidColorBrush DefaultStrokeColor = Brushes.Black;

        public VisualPoint(Point point, Canvas canvas): base(canvas)
        {
            _point = point;
        }

        public Point GetPoint() => _point;

        public void Draw(int size = 18, SolidColorBrush? color = null, int strokeThickness = 0, SolidColorBrush? strokeColor = null)
        {
            float rad = size / 2;

            shape = new Ellipse()
            {
                Fill = color ?? DefaultFillColor,
                Width = size,
                Height = size,
                RenderTransformOrigin = new System.Windows.Point(0.5, 0.5),
                StrokeThickness = 2,
                Stroke = strokeColor ?? DefaultStrokeColor
            };
            Canvas.SetLeft(shape, this._point.coord.X - rad);
            Canvas.SetTop(shape, this._point.coord.Y - rad);

            AddToCanvas();
        }

        public void Selected()
        {
            if (shape != null)
            {
                shape.StrokeThickness = 2;
                shape.Stroke = Brushes.Yellow;
            }
        }

        public void Hover()
        {
            if (shape is not null)
            {
                shape.Fill = Brushes.OrangeRed;
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

