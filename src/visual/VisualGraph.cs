﻿using selfdrivingcar.src.math;
using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;

namespace selfdrivingcar.src.visual
{
    internal class VisualGraph
    {
        public readonly Graph _graph;
        private readonly Canvas _canvas;
        public List<VisualPoint> VisualPoints { get; set; }
        public List<VisualSegment> VisualSegments { get; set; }

        private VisualPoint? SelectedPoint;
        private VisualPoint? HoveredPoint;
        private bool Dragging = false;

        public VisualGraph(Canvas canvas, Graph graph)
        {
            this._canvas = canvas;
            this._graph = graph;

            VisualPoints = _graph.Points.Select(x=> new VisualPoint(x, canvas)).ToList();
            VisualSegments = _graph.Segments.Select(x => new VisualSegment(x, canvas)).ToList();

            this._canvas.MouseDown += _canvas_MouseDown;
            this._canvas.MouseUp += _canvas_MouseUp;
            this._canvas.MouseMove += _canvas_MouseMove;
        }

        

        private void _canvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var position = e.GetPosition(this._canvas);
            var mouse = new Point((float)position.X, (float)position.Y);

            var nearest = Utils.GetNearestPoint(mouse, VisualPoints, 30);

            if (nearest is not null)
            {
                HoveredPoint?.Hover(false);
                HoveredPoint = nearest;
                HoveredPoint.Hover(true);
            }
            else
            {
                HoveredPoint?.Hover(false);
                HoveredPoint = null;
            }

            if (Dragging)
            {
                SelectedPoint!.UpdatePosition(mouse);
                foreach (var seg in VisualSegments)
                {
                    if (seg.GetSegment().Includes(SelectedPoint.GetPoint()))
                    {
                        seg.UpdatePosition();
                    }
                }
            }

        }
        private void _canvas_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Dragging = false;
        }
        private void _canvas_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Debug.WriteLine($"mouse down!");
            if (e.ChangedButton == System.Windows.Input.MouseButton.Right) // RIGHT CLICK (remove hovered point)
            {
                if (HoveredPoint is not null)
                {
                    RemovePoint(HoveredPoint.GetPoint());
                    if (SelectedPoint is not null && SelectedPoint.GetPoint().Equals(HoveredPoint.GetPoint()))
                        SelectedPoint = null;
                    HoveredPoint = null;
                }
            }
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left) //LEFT CLICK (new points)
            {  
                var position = e.GetPosition(this._canvas);
                var mouse = new Point((float)position.X, (float)position.Y);

                SelectedPoint?.Selected(false);
                if (HoveredPoint is not null)
                {
                    if (SelectedPoint is not null)
                    {
                        TryAddSegment(new Segment(SelectedPoint.GetPoint(), HoveredPoint.GetPoint()));
                    }
                    SelectedPoint = HoveredPoint;
                    SelectedPoint.Selected(true);
                    Dragging = true;
                    return;
                }

                TryAddPoint(mouse);

                if (SelectedPoint is not null)
                {
                    TryAddSegment(new Segment(SelectedPoint.GetPoint(), mouse));
                }
                SelectedPoint = this.VisualPoints.Last();
                SelectedPoint.Selected(true);
            }

        }

        public void Draw()
        {
            foreach (var seg in VisualSegments)
            {
                seg.Draw();
            }

            foreach (var point in VisualPoints)
            {
                point.Draw();
            }

            SelectedPoint?.Selected(true);
        }

        public void UnDraw()
        {
            foreach (var seg in VisualSegments)
            {
                seg.UnDraw();
            }

            foreach (var point in VisualPoints)
            {
                point.UnDraw();
            }
        }

        public bool TryAddPoint(Point point)
        {
            bool success = _graph.TryAddPoint(point);
            if (success)
            {
                var newPoint = new VisualPoint(point, _canvas); 
                VisualPoints.Add(newPoint);
                newPoint.Draw();
                return true;
            }
            
            return false;
        }

        public bool TryAddSegment(Segment segment)
        {
            bool success = _graph.TryAddSegment(segment);
            if (success)
            {
                var newSegment = new VisualSegment(segment, _canvas);
                VisualSegments.Add(newSegment);
                newSegment.Draw();

                return true;
            }
            return false;
        }
        public void RemoveSegment(int index)
        {
            this._graph.RemoveSegment(index);
            var remove = this.VisualSegments[index];
            this.VisualSegments.RemoveAt(index);
            remove.RemoveFromCanvas();
        }

        public void RemoveSegment(Segment seg)
        {
            int idx = this._graph.RemoveSegment(seg);
            if (idx != -1) {
                var remove = this.VisualSegments[idx];
                this.VisualSegments.RemoveAt(idx);

                remove.RemoveFromCanvas();
            }
        }

        public void RemovePoint(Point point)
        {
            int idx = _graph.RemovePoint(point);
            if (idx != -1)
            {
                var segs = _graph.GetSegmentsWithPoint(point); 
                foreach (var s in segs)
                {
                    RemoveSegment(s);
                }

                var remove = this.VisualPoints.FirstOrDefault(x=> x.GetPoint().Equals(point)); 
                if (remove is not null) { 
                    remove.RemoveFromCanvas();
                    this.VisualPoints.Remove(remove);
                    _canvas.UpdateLayout();
                }
            }
        }

        public void Dispose()
        {
            this._graph.Dispose();

            UnDraw();

            this.VisualPoints.Clear();
            this.VisualSegments.Clear();
        }
    }
}
