using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using selfdrivingcar.src.world;

namespace selfdrivingcar.src.visual
{
    internal class VisualPolyGon : VisualBase<Polygon>
    {
        public PolygonG _poly { get; private set; }
        public VisualPolyGon(Canvas canvas, PolygonG poly) : base(canvas)
        {
             this._poly = poly;
        }

        /// <summary>
        /// Update polygon points and update shape points in ordet to change automatically the canvas representation is already added to canvas
        /// </summary>
        /// <param name="points"></param>
        public void UpdatePoly(PolygonG poly)
        {
            this._poly = poly;
            if (shape is not null) { 
                shape.Points.Clear();
                poly.Points.ForEach(p => shape.Points.Add(p.ToWindowsPoint()));
            }

        }
        public void Draw(double strokeThickness , SolidColorBrush strokeColor, SolidColorBrush fillColor)
        {
            shape = new Polygon()
            {
                Stroke = strokeColor,
                StrokeThickness = strokeThickness,
                Fill = fillColor
            };

            foreach (var p in _poly.Points)
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
