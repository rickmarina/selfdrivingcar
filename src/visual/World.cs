using selfdrivingcar.src.items;
using selfdrivingcar.src.math;
using selfdrivingcar.src.world;
using System.Diagnostics;
using System.Numerics;
using System.Windows.Controls;
using System.Windows.Media;
using static selfdrivingcar.src.world.Enums;

namespace selfdrivingcar.src.visual
{
    internal class World
    {
        private ViewPort _viewPort;
        private VisualGraph _visualGraph;
        private string oldGraphHash = string.Empty;
        private readonly Canvas _canvas;
        private ScaleTransform _scale;
        private TranslateTransform _translate;
        private readonly WorldSettings _settings;
        private VisualPoint _viewPoint;

        //Procedural road path 
        //private List<VisualSegment> _roadBorders = [];
        private VisualPath _roadPath;

        //Procedural buildings 
        private List<Building> _buildings = []; 

        //Procedural trees
        private List<Tree> _trees = [];

        public World(MainWindow window, Canvas canvas, WorldSettings settings, ScaleTransform scaleT, TranslateTransform translateT)
        {
            _canvas = canvas;
            _settings = settings;
            _viewPort = new ViewPort(_canvas, window.ActualWidth, window.ActualHeight);
            _visualGraph = new VisualGraph(this, _viewPort, new Graph([], []), settings);

            _scale = scaleT; 
            _translate = translateT;

            _viewPort.ZoomChanged += _viewPort_ZoomChanged;
            _viewPort.OffsetChanged += _viewPort_OffsetChanged;

            _viewPort.SetOffset(new System.Numerics.Vector2((float)-_canvas.ActualWidth / 2, (float)-_canvas.ActualHeight / 2));

            _translate.X = -_canvas.ActualWidth / 2;
            _translate.Y = -_canvas.ActualHeight / 2;

            _roadPath = new(_canvas, []);
            _viewPoint = new(new Point(0, 0), _canvas);
            _viewPoint.Draw(color: BrushesUtils.Red);
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

            string newhash = _visualGraph.GetHash();

            Debug.WriteLine($"viewport: {_viewPort.Offset} width/height: {_viewPort.WindowSize}");

            Vector2 viewPoint = _viewPort.GetViewPoint();

            if (newhash != oldGraphHash) { 
                var roadSegments = PolygonG.Union(_visualGraph.VisualSegments.Where(x => x.HasEnvelope).Select(x => x.Envelope.Poly).ToList());
                //_roadBorders.ForEach(x => x.UnDraw());
                //_roadBorders = roadSegments.Select(x => new VisualSegment(x, _canvas, _settings, false)).ToList();
                //_roadBorders.ForEach(x => x.Draw(width: 4, color: BrushesUtils.White));

                _roadPath.Undraw();
                _roadPath = new VisualPath(_canvas, roadSegments);
                _roadPath.Draw(BrushesUtils.White, _settings.RoadPathStrokeThickness);

                //Generate buildings 
                _buildings.ForEach(x => x.UnDraw()); 
                _buildings = GenerateBuildings();
                _buildings.ForEach(x => x.Draw(viewPoint));

                //Generate trees
                _trees.ForEach(x => x.UnDraw());
                _trees = GenerateTrees();
                _trees.ForEach(x => x.Draw());

                oldGraphHash = newhash;
            }

            //Reorder items in order to get correct z-index
            List<IItem> items = [.. _trees, .. _buildings];
            items.Sort((a, b) => b.GetBase().DistanceToPoint(new Point(viewPoint)).CompareTo(a.GetBase().DistanceToPoint(new Point(viewPoint))));

            //Update trees and buildings according to viewpoint (adjust z-index)
            int? zindexBase = (int)ZINDEXES.BUILDINGS_TREES;
            items.ForEach(i =>
            {
                i.UpdateViewPoint(viewPoint, zindexBase); 
                zindexBase += 10;
            });


            //_trees.ForEach(x => x.UpdateViewPoint(viewPoint));

            ////Update buildings according to viewpoint (adjust z-index)
            //_buildings.ForEach(x => x.UpdateViewPoint(viewPoint));

        }

        private List<Building> GenerateBuildings()
        {
            var tmpEnvelopes = new List<Envelope>(); 
            foreach (var seg in _visualGraph.VisualSegments)
            {
                tmpEnvelopes.Add(
                    new Envelope(seg.GetSegment(), _settings.RoadWidth + _settings.BuildingWidth + _settings.BuildingSpacing * 2, _canvas, roundness: _settings.RoadRoundness)
                    );
            }

            var guides = PolygonG.Union(tmpEnvelopes.Select(x=> x.Poly).ToList()!);
            guides = guides.Where(x=> x.Length() > _settings.BuildingMinLength).ToList();

            var supports = new List<Segment>(); 
            foreach (var seg in guides)
            {
                float len = seg.Length() + _settings.BuildingSpacing;
                int buildingCount = (int)Math.Floor(len/ (_settings.BuildingMinLength +  _settings.BuildingSpacing));
                float buildingLength = len / buildingCount - _settings.BuildingSpacing;

                Vector2 dir = seg.DirectionVector();

                Point q1 = seg.PointA;
                Point q2 = new Point(Vector2.Add(q1.coord, Vector2.Multiply(dir, buildingLength)));

                supports.Add(new Segment(q1, q2));

                for (int i= 2; i<= buildingCount; i++)
                {
                    q1 = new Point(Vector2.Add(q2.coord, Vector2.Multiply(dir, _settings.BuildingSpacing)));
                    q2 = new Point(Vector2.Add(q1.coord, Vector2.Multiply(dir, buildingLength)));
                    supports.Add(new Segment(q1, q2));
                }
            }

            List<Envelope> bases = new(); 
            foreach (var seg in supports)
            {
                bases.Add(new Envelope(seg, _settings.BuildingWidth, _canvas));
            }

            float eps = 0.001f;
            for (int i = 0; i < bases.Count-1; i++)
            {
                for (int j= i+1; j< bases.Count; j++)
                {
                    if (bases[i].Poly!.IntersectsPoly(bases[j].Poly!) ||
                        bases[i].Poly!.DistanceToPoly(bases[j].Poly!) < _settings.BuildingSpacing - eps
                        ) {
                        bases.RemoveAt(j);
                        j--;
                    }
                }
            }

            return bases.Select(x=> new Building(_canvas, _settings, x.Poly!)).ToList();
        }

        private List<Tree> GenerateTrees()
        {
            //var points = [.. _roadPath.Segments.SelectMany(x => new[] { x.PointA, x.PointB }).ToList(), ]
            var points = _roadPath.Segments.SelectMany(x => new[] { x.PointA, x.PointB }).ToList();
            points = [.. points, .. _buildings.SelectMany(x => x.GetBasePoly().Points)];

            float left = points.Min(x => x.coord.X);
            float right = points.Max(x=> x.coord.X);
            float top = points.Min(x => x.coord.Y);
            float bottom = points.Max(x => x.coord.Y);

            List<PolygonG?> illegalPolys = [.. _buildings.Select(x => x.GetBasePoly()) , .. _visualGraph.VisualSegments.Select(x => x.Envelope.Poly)];

            var trees = new List<Tree>();
            int tryCount = 0;

            while (tryCount < _settings.TryCountTrees)
            {
                var p = new Point(Utils.Lerp(left, right, Random.Shared.NextSingle()), Utils.Lerp(bottom, top, Random.Shared.NextSingle()));

                bool keep = true;
                if (illegalPolys.Any(x => (x.ContainsPoint(p) || x.DistanceToPoint(p) < _settings.TreeSize/2)) || trees.Any(x=> Utils.Distance(x.Center, p) < _settings.TreeSize))
                    keep = false;

                // Avoiding trees in the middle of nowhere
                if (keep)
                {
                    bool closeToSomething = illegalPolys.Any(x => x.DistanceToPoint(p) < _settings.TreeSize * 2);
                    keep = closeToSomething;
                }

                if (keep)
                {
                    trees.Add(new Tree(_canvas, p, _settings.TreeSize, _settings, _viewPort.GetViewPoint()));
                    tryCount = 0;
                }
                else
                    tryCount++;
            }

            return trees;
        }

        public void Dispose()
        {
            _visualGraph.Dispose();
        }

        private void _viewPort_OffsetChanged(object? sender, Vector2 e)
        {
            _translate.X += e.X;
            _translate.Y += e.Y;

            //Debug.WriteLine($"{e.X} {e.Y}");
            _viewPoint.UpdatePosition(new Point(_viewPort.GetViewPoint()));
            Generate();

            //Debug.WriteLine($"viewpoint:{_viewPort.GetViewPoint()}");
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
