using selfdrivingcar.src.math;
using selfdrivingcar.src.world;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace selfdrivingcar.src.visual
{
    internal class VisualGraph
    {
        public readonly Graph _graph;
        private readonly ViewPort _viewPort; 
        private readonly Canvas _canvas;
        [JsonIgnore]
        public List<VisualPoint> VisualPoints { get; set; }
        [JsonIgnore]
        public List<VisualSegment> VisualSegments { get; set; }
        private List<VisualSegment> _intersectionSegments;

        private Point? MousePosition = null; // mouse position on the canvas
        private VisualPoint? SelectedPoint; // point selected
        private VisualPoint? HoveredPoint; // point hovered
        private VisualSegment? IntentionSegment; // segment display intention if any point is selected and mouse is moving
        private bool Dragging = false;
        private readonly WorldSettings _settings;
        private readonly World _world;
        public VisualGraph(World world, ViewPort viewPort, Graph graph, WorldSettings settings)
        {
            this._world = world;
            this._viewPort = viewPort;
            this._canvas = viewPort._canvas;
            this._graph = graph;
            this._settings = settings;

            VisualPoints = _graph.Points.Select(x=> new VisualPoint(x, _canvas)).ToList();
            VisualSegments = _graph.Segments.Select(x => new VisualSegment(x, _canvas, settings, true)).ToList();
            
            IntentionSegment = new VisualSegment(new Segment(new Point(0, 0), new Point(0,0)), _canvas, settings, false);
            IntentionSegment.Draw(strokedasharray: [4,2]);

            _intersectionSegments = new();

            //Canvas events
            this._canvas.MouseDown += _canvas_MouseDown;
            this._canvas.MouseUp += _canvas_MouseUp;
            this._canvas.MouseMove += _canvas_MouseMove;
            
        }
        public void Load(RootJson? root)
        {
            if (root is not null) { 
                root._graph.Points.ForEach(x =>
                    TryAddPoint(new Point(x.coord.X, x.coord.Y))
                );
                root._graph.Segments.ForEach(x => {
                    var pA = VisualPoints.FirstOrDefault(v => v.GetPoint().Equals(new Point(x.PointA.coord.X, x.PointA.coord.Y)));
                    var pB = VisualPoints.FirstOrDefault(v => v.GetPoint().Equals(new Point(x.PointB.coord.X, x.PointB.coord.Y)));
                    TryAddSegment(new Segment(pA.GetPoint(), pB.GetPoint()));
                }
                );

                //Envelope envelope = new Envelope(VisualSegments[0].GetSegment(), 20, _canvas);
                //envelope.Draw();
            }
        }
        

        private void _canvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var position = this._viewPort.GetMouse(e);
            MousePosition = new Point((float)position.X, (float)position.Y);

            var nearest = Utils.GetNearestPoint(MousePosition, VisualPoints, 30 * this._viewPort.Zoom);

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
                var affectedSegments = new List<VisualSegment>();

                SelectedPoint!.UpdatePosition(MousePosition);
                foreach (var seg in VisualSegments)
                {
                    if (Utils.DistanceFromPointToSegment(MousePosition, seg.GetSegment()) < 100)
                    {
                        affectedSegments.Add(seg);
                    }

                    if (seg.GetSegment().Includes(SelectedPoint.GetPoint()))
                    {
                        seg.UpdatePosition();
                    }
                    if (seg.HasEnvelope) // Reset poly segments before we break again 
                    {
                        seg.Envelope.UpdatePoly();
                    }
                }

                Debug.WriteLine($"Affected segments: {affectedSegments.Count}");
                _world.Generate();
                
            }

            if (SelectedPoint is not null && IntentionSegment is not null)
            {
                IntentionSegment.GetSegment().PointA = SelectedPoint.GetPoint();
                IntentionSegment.GetSegment().PointB = HoveredPoint is not null ? HoveredPoint.GetPoint() :  MousePosition;
                IntentionSegment.UpdatePosition();
            } else
            {
                IntentionSegment?.ResetToOrigin();
            }
        }

        private void _canvas_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Debug.WriteLine($"Mouse UP");
            Dragging = false;
        }
        private void _canvas_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Debug.WriteLine($"mouse down!");
            if (e.ChangedButton == System.Windows.Input.MouseButton.Right) // RIGHT CLICK (remove hovered point)
            {
                if (SelectedPoint is not null)
                {
                    SelectedPoint?.Selected(false);
                    SelectedPoint = null;
                }
                else if (HoveredPoint is not null)
                {
                    RemovePoint(HoveredPoint.GetPoint());
                    HoveredPoint = null;
                } 
                
            }
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left) //LEFT CLICK (new points)
            {  
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

                if (MousePosition != null) { 
                    TryAddPoint(MousePosition);

                    if (SelectedPoint is not null)
                    {
                        TryAddSegment(new Segment(SelectedPoint.GetPoint(), MousePosition));
                    }
                    SelectedPoint = this.VisualPoints.Last();
                    SelectedPoint.Selected(true);
                }
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
                var newSegment = new VisualSegment(segment, _canvas, _settings, true);
                VisualSegments.Add(newSegment);
                newSegment.Draw(color: BrushesUtils.White, strokedasharray: [5, 5]);

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

                remove.UnDraw();
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

            SelectedPoint = null;
            HoveredPoint = null;
        }
    }
}
