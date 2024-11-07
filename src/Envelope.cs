﻿using selfdrivingcar.src.math;
using selfdrivingcar.src.visual;
using System.Diagnostics;
using System.Numerics;
using System.Windows.Controls;
using System.Windows.Media;

namespace selfdrivingcar.src
{
    internal class Envelope : VisualBase<System.Windows.Shapes.Polygon>
    {
        private Segment _skeleton;
        private readonly int _width;
        private PolygonG _polygon;

        public Envelope(Segment skeleton, int width, Canvas canvas) : base(canvas)
        {
            _skeleton = skeleton;
            _width = width;
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
            double step = Math.PI / 10;
            for(double i=alpha_ccw; i<= alpha_cw; i+= step)
            {
                roundPoints.Add(Utils.Translate(p1, i, radius));
            }
            for (double i = alpha_ccw; i <= alpha_cw; i += step)
            {
                roundPoints.Add(Utils.Translate(p2, Math.PI + i, radius));
            }
            return new PolygonG(roundPoints);

        }
        
        public void Draw()
        {
            _polygon = GeneratePolygon();

            shape = new System.Windows.Shapes.Polygon()
            {
                StrokeThickness = 2,
                Stroke = Brushes.Blue,
                Fill = new SolidColorBrush(Color.FromArgb(30, 0, 0, 255)),
            };

            foreach (var p in _polygon._points)
            {
                shape.Points.Add(new System.Windows.Point(p.coord.X, p.coord.Y));
            }
            shape.IsEnabled = false;
            shape.IsHitTestVisible = false;
            Canvas.SetZIndex(shape, -10);

            AddToCanvas();
        }

        public void UnDraw()
        {
            RemoveFromCanvas();
        }

    }
}
