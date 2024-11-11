using selfdrivingcar.src.math;
using selfdrivingcar.src.visual;
using System.Diagnostics;
using System.Numerics;
using System.Windows.Controls;
using System.Windows.Media;
using static selfdrivingcar.src.world.Enums;

namespace selfdrivingcar.src
{
    internal class Envelope : VisualBase<System.Windows.Shapes.Polygon>
    {
        private Segment _skeleton;
        private readonly int _width;
        private readonly int _roundness;
        public PolygonG? Poly { get; private set; }

        public Envelope(Segment skeleton, int width, Canvas canvas, int roundness = 1) : base(canvas)
        {
            _skeleton = skeleton;
            _width = width;
            _roundness = roundness;
        }

        private PolygonG GeneratePolygon()
        {
            int radius = _width / 2;
            var p1 = _skeleton.PointA;
            var p2 = _skeleton.PointB;

            var subs = Vector2.Subtract(p1.coord, p2.coord);
            
            double alpha = Utils.Angle(new Point(subs.X, subs.Y));
            double alpha_cw = alpha + Math.PI / 2;
            double alpha_ccw = alpha - Math.PI / 2;

            //Point p1_ccw = Utils.Translate(p1, alpha_ccw, radius);
            //Point p2_ccw = Utils.Translate(p2, alpha_ccw, radius);
            //Point p2_cw = Utils.Translate(p2, alpha_cw, radius);
            //Point p1_cw = Utils.Translate(p1, alpha_cw, radius);

            //Generate points from p1 around then continue with p2 , this way we generate a polygon that groups this two points with rounder borders
            List<Point> roundPoints = new List<Point>();
            double step = Math.PI / Math.Max(1, _roundness);
            double eps = step / 2; 
            for(double i=alpha_ccw; i<= alpha_cw + eps; i+= step)
            {
                roundPoints.Add(Utils.Translate(p1, i, radius));
            }
            for (double i = alpha_ccw; i <= alpha_cw + eps; i += step)
            {
                roundPoints.Add(Utils.Translate(p2, Math.PI + i, radius));
            }
            return new PolygonG(roundPoints);

        }

        public void UpdatePoly()
        {
            Poly = GeneratePolygon();
        }
        
        public void Draw()
        {
            Poly = GeneratePolygon();

            shape = new System.Windows.Shapes.Polygon()
            {
                StrokeThickness = 13,
                Stroke = BrushesUtils.BrushRoad,
                Fill = BrushesUtils.BrushRoad,
               
            };

            foreach (var p in Poly.Points)
            {
                shape.Points.Add(new System.Windows.Point(p.coord.X, p.coord.Y));
            }
            shape.IsEnabled = false;
            shape.IsHitTestVisible = false;

            AddToCanvas(ZINDEXES.ROAD);
        }

        public void UnDraw()
        {
            RemoveFromCanvas();
        }

    }
}
