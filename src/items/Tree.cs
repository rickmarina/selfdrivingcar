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
        private Vector2 top;
        private VisualSegment segment;

        public Tree(Canvas canvas, Point center, float size, WorldSettings settings, Vector2 viewPoint)
        {
            this._canvas = canvas;
            this.Center = center;
            this.size = size; //size of the base
            this.settings = settings;
            pTree1 = new VisualPoint(center, _canvas, size);
            Vector2 diff = Vector2.Subtract(center.coord, viewPoint);
            top = Vector2.Add(center.coord, diff);
            segment = new VisualSegment(new Segment(center, new Point(top)), _canvas, settings, false);
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
