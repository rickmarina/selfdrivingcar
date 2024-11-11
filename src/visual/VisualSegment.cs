using selfdrivingcar.src.world;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace selfdrivingcar.src.visual
{
    internal class VisualSegment : VisualBase<Line>
    {
        private readonly Segment _segment;
        private static readonly SolidColorBrush DefaultStrokeColor = Brushes.Black;
        public Envelope Envelope { get; private set; }
        public bool HasEnvelope { get; private set; }

        public VisualSegment(Segment segment, Canvas canvas, WorldSettings settings, bool hasEnvelope) : base(canvas)
        {
            _segment = segment;
            HasEnvelope = hasEnvelope;

            if (HasEnvelope)
                Envelope = new Envelope(segment, settings.RoadWidth, canvas, settings.RoadRoundness);
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
                Y2 = _segment.PointB.coord.Y
            };

            if (strokedasharray != null)
            {
                shape.StrokeDashArray = strokedasharray;
            }
            if (HasEnvelope)
                Envelope.Draw();

            AddToCanvas(Enums.ZINDEXES.ROAD_LINES);
        }

        public void UpdatePosition()
        {
            if (HasEnvelope)
            {
                Envelope?.UnDraw();
                Envelope?.Draw();
            }

            if (shape != null)
            {
                shape.X1 = _segment.PointA.coord.X;
                shape.Y1 = _segment.PointA.coord.Y;
                shape.X2 = _segment.PointB.coord.X;
                shape.Y2 = _segment.PointB.coord.Y;
            }
        }

        public void UnDraw() {
            if (HasEnvelope)
                Envelope?.RemoveFromCanvas();

            RemoveFromCanvas();
        }

        public void ResetToOrigin()
        {
            if (shape != null)
            {
                shape.X1 = 0;
                shape.Y1 = 0;
                shape.X2 = 0;
                shape.Y2 = 0;
            }

            if (HasEnvelope && Envelope.addedToCanvas)
                Envelope.RemoveFromCanvas();
        }
        
    }
}
