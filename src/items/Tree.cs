using selfdrivingcar.src.math;
using selfdrivingcar.src.visual;
using selfdrivingcar.src.world;
using System.Diagnostics;
using System.Numerics;
using System.Windows.Controls;
using System.Windows.Media;

namespace selfdrivingcar.src.items
{
    internal class Tree
    {
        private readonly Canvas _canvas;
        public Point Center { get; }
        private readonly float size;
        private readonly float heightCoef;
        private readonly WorldSettings settings;
        private List<VisualPolyGon> levelsTree;
        //private VisualPolyGon treeBase; 

        public Tree(Canvas canvas, Point center, float size, WorldSettings settings, Vector2 viewPoint, float HeightCoef = 0.3f)
        {
            this._canvas = canvas;
            this.Center = center;
            this.size = size; //size of the base
            this.settings = settings;
            this.heightCoef = HeightCoef;

            levelsTree = new();
            Vector2 top = GetTopVector(viewPoint);

            for (int level = 0; level < settings.TreeLevels; level++)
            {
                float t = level / (float)(settings.TreeLevels - 1);
                Point point = new Point(Vector2.Lerp(Center.coord, top, t));
                levelsTree.Add(new VisualPolyGon(_canvas, GenerateLevel(point, Utils.Lerp(size, 40, t))));
            }
            //treeBase = new VisualPolyGon(_canvas, GenerateLevel(Center, size));
        }

        private Vector2 GetTopVector(Vector2 viewPoint)
        {
            Vector2 diff = Vector2.Subtract(Center.coord, viewPoint) * heightCoef;
            Vector2 top = Vector2.Add(Center.coord, diff);
            
            //Limit tree height if it is too far
            if (diff.Length() > 220)
                top = Vector2.Add(Center.coord, Vector2.Normalize(diff) * 220);

            return top;
        }

        public void UpdateViewPoint(Vector2 viewPoint)
        {
            Vector2 top = GetTopVector(viewPoint);

            for (int level = 0; level < settings.TreeLevels; level++)
            {
                float t = level / (float)(settings.TreeLevels - 1);
                Point point = new Point(Vector2.Lerp(Center.coord, top, t));
                levelsTree[level].UpdatePoly(GenerateLevel(point, Utils.Lerp(size, 40, t)));
            }
            //treeBase.UpdatePoly(GenerateLevel(Center, size));
        }

        public PolygonG GenerateLevel(Point point, float size)
        {
            List<Point> points = [];
            float rad = size / 2; 

            for (double a =0; a < Math.PI * 2; a+= Math.PI/16)
            {
                float kindOfRandom = (float)Math.Pow(Math.Cos(((a + Center.coord.X) * size) % 17), 2);
                float noisyRad = rad * Utils.Lerp(0.5f, 1f, kindOfRandom);
                points.Add(Utils.Translate(point, a, noisyRad));
            }

            return new PolygonG(points);
        }

        public void Draw()
        {
            for (int level = 0; level < settings.TreeLevels; level++)
            {
                float t = level / (float)(settings.TreeLevels - 1);
                SolidColorBrush treeColor = BrushesUtils.TreeColorLerp(t);
                levelsTree[level].Draw(0, treeColor, treeColor);
            }
            //treeBase.Draw(2, BrushesUtils.White, BrushesUtils.BlueTransparent(50));
        }

        public void UnDraw()
        {
            levelsTree.ForEach(x => x.UnDraw());
        }
    }
}
