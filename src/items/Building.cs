using selfdrivingcar.src.visual;
using System.Numerics;
using System.Windows.Controls;
using static selfdrivingcar.src.world.Enums;

namespace selfdrivingcar.src.items
{
    internal class Building
    {
        private readonly Canvas canvas;
        private readonly PolygonG buildingBase;
        private readonly float heightCoef;

        private VisualPolyGon vBase;
        private VisualPolyGon vCeiling;
        private List<VisualPolyGon> vSides;

        public Building(Canvas canvas, PolygonG buildingBase, float HeightCoef = 0.3f) 
        {
            this.canvas = canvas;
            this.buildingBase = buildingBase;
            this.heightCoef = HeightCoef;

            vBase = new VisualPolyGon(canvas, buildingBase);
            vCeiling = new VisualPolyGon(canvas, new PolygonG([]));
            vSides = [];
        }

        public PolygonG GetBasePoly() => vBase._poly;

        public void UpdateViewPoint(Vector2 viewPoint)
        {
            var topPoints = vBase._poly.Points.Select(p => new Point(Vector2.Add(p.coord, Vector2.Multiply(Vector2.Subtract(p.coord, viewPoint), this.heightCoef)))).ToList();

            vCeiling.UpdatePoly(new PolygonG(topPoints));


            for (int i = 0; i < vBase._poly.Points.Count; i++)
            {
                int nextI = (i + 1) % vBase._poly.Points.Count;
                vSides[i].UpdatePoly(new PolygonG([vBase._poly.Points[i], vBase._poly.Points[nextI], topPoints[nextI], topPoints[i]]));
            }
            vSides.Sort((a, b) => a._poly.DistanceToPoint(new Point(viewPoint)).CompareTo(b._poly.DistanceToPoint(new Point(viewPoint))));

            //update zindex
            vBase.SetZIndex((int)ZINDEXES.BUILDINGS);
            for (int i = 0; i < vSides.Count; i++)
            {
                vSides[i].SetZIndex((int)ZINDEXES.BUILDINGS + vSides.Count - i);
            }
            vCeiling.SetZIndex((int)ZINDEXES.BUILDINGS + vSides.Count + 1);


        }

        public void Draw(Vector2 viewPoint)
        {
            var topPoints = vBase._poly.Points.Select(p => new Point(Vector2.Add(p.coord, Vector2.Multiply(Vector2.Subtract(p.coord, viewPoint), this.heightCoef)))).ToList();

            PolygonG ceiling = new PolygonG(topPoints);
            vCeiling = new VisualPolyGon(this.canvas, ceiling);

            for (int i = 0; i < vBase._poly.Points.Count; i++)
            {
                int nextI = (i+1) % vBase._poly.Points.Count;
                PolygonG sidePoly = new PolygonG([vBase._poly.Points[i], vBase._poly.Points[nextI], topPoints[nextI], topPoints[i]]);
                vSides.Add(new VisualPolyGon(canvas, sidePoly));
            }

            vSides.Sort((a, b) => a._poly.DistanceToPoint(new Point(viewPoint)).CompareTo(b._poly.DistanceToPoint(new Point(viewPoint))));

            vBase.Draw(1, BrushesUtils.Gray, BrushesUtils.White);
            vSides.ForEach(s=> s.Draw(1, BrushesUtils.Gray, BrushesUtils.White));
            vCeiling.Draw(1, BrushesUtils.Gray, BrushesUtils.White);
        }

        public void UnDraw()
        {
            vBase.UnDraw();
            vSides.ForEach(s => s.UnDraw());
            vCeiling.UnDraw();
        }
    }
}
