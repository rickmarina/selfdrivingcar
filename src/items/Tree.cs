using selfdrivingcar.src.visual;
using selfdrivingcar.src.world;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace selfdrivingcar.src.items
{
    internal class Tree
    {
        private readonly Canvas _canvas;
        public Point Center { get; }
        private readonly float size;
        private readonly WorldSettings settings;
        private VisualPoint pTree1;
        private VisualSegment segment;

        public Tree(Canvas canvas, Point center, float size, WorldSettings settings, Vector2 viewPoint)
        {
            this._canvas = canvas;
            this.Center = center;
            this.size = size; //size of the base
            this.settings = settings;
            pTree1 = new VisualPoint(center, _canvas, size);

            segment = new VisualSegment(new Segment(Center, new Point(GetTopVector(viewPoint))), _canvas, settings, false);
        }

        private Vector2 GetTopVector(Vector2 viewPoint)
        {
            Vector2 diff = Vector2.Subtract(Center.coord, viewPoint);
            Vector2 top = Vector2.Add(Center.coord, diff);
            return top;
        }

        public void UpdateViewPoint(Vector2 viewPoint)
        {
            segment.GetSegment().PointB.coord = GetTopVector(viewPoint);
            segment.UpdatePosition();
        }

        public void Draw()
        {
            pTree1.Draw(color: BrushesUtils.TreeColor);
            segment.Draw();
        }

        public void UnDraw()
        {
            pTree1.UnDraw();
            segment.UnDraw();
        }
    }
}
