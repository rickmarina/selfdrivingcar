﻿using System.Windows.Controls;
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

        public Segment GetSegment() => _segment;

        public void Draw(int width = 2, SolidColorBrush? color = null, DoubleCollection? strokedasharray = null)
        {
            shape = new Line()
            {
                StrokeThickness = width,
                Stroke = color ?? DefaultStrokeColor,
                X1 = _segment.PointA.coord.X,
                Y1 = _segment.PointA.coord.Y,
                X2 = _segment.PointB.coord.X,
                Y2 = _segment.PointB.coord.Y,
            };
            
            if (strokedasharray != null )
            {
                shape.StrokeDashArray = strokedasharray;
            }

            AddToCanvas();
        }

        public void UpdatePosition()
        {
            if (shape != null)
            {
                shape.X1 = _segment.PointA.coord.X;
                shape.Y1 = _segment.PointA.coord.Y;
                shape.X2 = _segment.PointB.coord.X;
                shape.Y2 = _segment.PointB.coord.Y;
            }
        }

        public void UnDraw() => RemoveFromCanvas();

        public void ResetToOrigin()
        {
            if (shape != null)
            {
                shape.X1 = 0;
                shape.Y1 = 0;
                shape.X2 = 0;
                shape.Y2 = 0;
            }
        }
        
    }
}
