using selfdrivingcar.src.world;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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


        public World(Canvas canvas, WorldSettings settings, ScaleTransform scaleT, TranslateTransform translateT)
        {
            _canvas = canvas;
            _viewPort = new ViewPort(_canvas);
            _visualGraph = new VisualGraph(_viewPort, new Graph([], []), settings);

            _scale = scaleT; 
            _translate = translateT;

            _viewPort.ZoomChanged += _viewPort_ZoomChanged;
            _viewPort.OffsetChanged += _viewPort_OffsetChanged;

            _viewPort.SetOffset(new System.Numerics.Vector2((float)-_canvas.ActualWidth / 2, (float)-_canvas.ActualHeight / 2));

            _translate.X = -_canvas.ActualWidth / 2;
            _translate.Y = -_canvas.ActualHeight / 2;
        }
        public VisualGraph GetVisualGraph() => this._visualGraph;

        public void Load(RootJson? root)
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
