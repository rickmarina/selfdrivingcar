using Microsoft.VisualBasic;
using selfdrivingcar.src.world;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace selfdrivingcar.src.visual
{
    internal class World
    {
        private ViewPort _viewPort;
        private VisualGraph _visualGraph;
        private readonly Canvas _canvas;
        private ScaleTransform _scale;
        private TranslateTransform _translate;
        private readonly WorldSettings _worldSettings;

        //Procedural road borders 
        private List<VisualSegment> _roadBorders = new();

        //Procedural buildings 
        private List<Envelope> _buildings = new(); 

        //Procedural trees


        public World(Canvas canvas, WorldSettings settings, ScaleTransform scaleT, TranslateTransform translateT)
        {
            _canvas = canvas;
            _worldSettings = settings;
            _viewPort = new ViewPort(_canvas);
            _visualGraph = new VisualGraph(this, _viewPort, new Graph([], []), settings);

            _scale = scaleT; 
            _translate = translateT;

            _viewPort.ZoomChanged += _viewPort_ZoomChanged;
            _viewPort.OffsetChanged += _viewPort_OffsetChanged;

            _viewPort.SetOffset(new System.Numerics.Vector2((float)-_canvas.ActualWidth / 2, (float)-_canvas.ActualHeight / 2));

            _translate.X = -_canvas.ActualWidth / 2;
            _translate.Y = -_canvas.ActualHeight / 2;
        }
        public VisualGraph GetVisualGraph() => this._visualGraph;

        public void Load(RootJson? root, bool generate)
        {
            if (root is not null)
            {
                root._graph.Points.ForEach(x =>
                    _visualGraph.TryAddPoint(new Point(x.coord.X, x.coord.Y))
                );
                root._graph.Segments.ForEach(x => {
                    var pA = _visualGraph.VisualPoints.FirstOrDefault(v => v.GetPoint().Equals(new Point(x.PointA.coord.X, x.PointA.coord.Y)));
                    var pB = _visualGraph.VisualPoints.FirstOrDefault(v => v.GetPoint().Equals(new Point(x.PointB.coord.X, x.PointB.coord.Y)));
                    _visualGraph.TryAddSegment(new Segment(pA.GetPoint(), pB.GetPoint()));
                    }
                );
            }
            if (generate)
            {
                Generate();
            }
        }
        /// <summary>
        /// Generate the virtual world:
        ///     1. Dynamic roads
        ///     
        /// </summary>
        public void Generate()
        {
            //Generate Road
            // Además de saber qué segmentos contienen el punto, recuperar la distancia entre el punto que se mueve y los segmentos para saber cuales tenemos que actualizar

            _roadBorders.ForEach(x => x.UnDraw());

            var roadSegments = PolygonG.Union(_visualGraph.VisualSegments.Where(x => x.HasEnvelope).Select(x => x.Envelope.Poly).ToList());
            var roadSegmentsVisual = roadSegments.Select(x => new VisualSegment(x, _canvas, _worldSettings, false)).ToList();

            _roadBorders = roadSegmentsVisual;
            roadSegmentsVisual.ForEach(x => x.Draw(width: 4, color: BrushesUtils.White));

            //Generate buildings 
            _buildings.ForEach(x => x.UnDraw()); 
            _buildings = GenerateBuildings();
            _buildings.ForEach(x => x.Draw(_worldSettings.BuidingStrokeThickness, fillBrush: _worldSettings.BuildingFillColor));

            //Generate trees
        }

        public List<Envelope> GenerateBuildings()
        {
            var tmpEnvelopes = new List<Envelope>(); 
            foreach (var seg in _visualGraph.VisualSegments)
            {
                tmpEnvelopes.Add(
                    new Envelope(seg.GetSegment(), _worldSettings.RoadWidth + _worldSettings.BuildingWidth + _worldSettings.BuildingSpacing * 2, _canvas, roundness: _worldSettings.RoadRoundness)
                    );
            }

            var guides = PolygonG.Union(tmpEnvelopes.Select(x=> x.Poly).ToList()!);
            guides = guides.Where(x=> x.Length() > _worldSettings.BuildingMinLength).ToList();

            var supports = new List<Segment>(); 
            foreach (var seg in guides)
            {
                float len = seg.Length() + _worldSettings.BuildingSpacing;
                int buildingCount = (int)Math.Floor(len/ (_worldSettings.BuildingMinLength +  _worldSettings.BuildingSpacing));
                float buildingLength = len / buildingCount - _worldSettings.BuildingSpacing;

                Vector2 dir = seg.DirectionVector();

                Point q1 = seg.PointA;
                Point q2 = new Point(Vector2.Add(q1.coord, Vector2.Multiply(dir, buildingLength)));

                supports.Add(new Segment(q1, q2));

                for (int i= 2; i<= buildingCount; i++)
                {
                    q1 = new Point(Vector2.Add(q2.coord, Vector2.Multiply(dir, _worldSettings.BuildingSpacing)));
                    q2 = new Point(Vector2.Add(q1.coord, Vector2.Multiply(dir, buildingLength)));
                    supports.Add(new Segment(q1, q2));
                }
            }

            List<Envelope> bases = new(); 
            foreach (var seg in supports)
            {
                bases.Add(new Envelope(seg, _worldSettings.BuildingWidth, _canvas));
            }

            

            for (int i = 0; i < bases.Count-1; i++)
            {
                for (int j= i+1; j< bases.Count; j++)
                {
                    if (bases[i].Poly.IntersectsPoly(bases[j].Poly)) {
                        bases.RemoveAt(j);
                        j--;
                    }
                }
            }

            return bases;
        }

        public void Dispose()
        {
            _visualGraph.Dispose();
        }

        private void _viewPort_OffsetChanged(object? sender, Vector2 e)
        {
            _translate.X += e.X;
            _translate.Y += e.Y;
        }

        private void _viewPort_ZoomChanged(object? sender, float e)
        {
            if (_viewPort is not null)
            {
                _scale.ScaleX = 1 / _viewPort.Zoom;
                _scale.ScaleY = 1 / _viewPort.Zoom;
            }
        }
    }
}
