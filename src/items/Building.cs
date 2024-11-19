using selfdrivingcar.src.math;
using selfdrivingcar.src.visual;
using selfdrivingcar.src.world;
using System.Numerics;
using System.Windows.Controls;
using static selfdrivingcar.src.world.Enums;

namespace selfdrivingcar.src.items
{
    internal class Building : IItem
    {
        private readonly Canvas canvas;
        private readonly PolygonG buildingBase;
        private readonly float heightCoef;
        private readonly WorldSettings _settings;

        private VisualPolyGon vBase;
        //Ceiling no more necessary if we have a roof
        //private VisualPolyGon vCeiling;
        private List<VisualPolyGon> vSides;
        private List<VisualPolyGon> vRoof;

        public Building(Canvas canvas, WorldSettings settings, PolygonG buildingBase, float HeightCoef = 0.07f) 
        {
            this.canvas = canvas;
            this._settings = settings;
            this.buildingBase = buildingBase;
            this.heightCoef = HeightCoef;

            vBase = new VisualPolyGon(canvas, buildingBase);
            //vCeiling = new VisualPolyGon(canvas, new PolygonG([]));
            vSides = [];
            vRoof = [];
        }

        public PolygonG GetBasePoly() => vBase._poly;

        public void UpdateViewPoint(Vector2 viewPoint, int? zindex)
        {
            int baseZindex = zindex ?? (int)ZINDEXES.BUILDINGS_TREES;

            var topPoints = vBase._poly.Points.Select(p => new Point(Vector2.Add(p.coord, Vector2.Multiply(Vector2.Subtract(p.coord, viewPoint), this.heightCoef)))).ToList();

            //vCeiling.UpdatePoly(new PolygonG(topPoints));
            
            for (int i = 0; i < vBase._poly.Points.Count; i++)
            {
                int nextI = (i + 1) % vBase._poly.Points.Count;
                vSides[i].UpdatePoly(new PolygonG([vBase._poly.Points[i], vBase._poly.Points[nextI], topPoints[nextI], topPoints[i]]));
            }
            vSides.Sort((a, b) => a._poly.DistanceToPoint(new Point(viewPoint)).CompareTo(b._poly.DistanceToPoint(new Point(viewPoint))));

            //update zindex
            vBase.SetZIndex(baseZindex);
            for (int i = 0; i < vSides.Count; i++)
            {
                vSides[i].SetZIndex(baseZindex + vSides.Count - i);
            }

            baseZindex += vSides.Count + 1;
            //vCeiling.SetZIndex((int)ZINDEXES.BUILDINGS_TREES + vSides.Count + 1);

            // Roof
            var p1 = Vector2.Lerp(topPoints[0].coord, topPoints[1].coord, 0.5f);
            var p2 = Vector2.Lerp(topPoints[2].coord, topPoints[3].coord, 0.5f);
            var diff = (p1 - viewPoint) * 0.15f;
            var diff2 = (p2 - viewPoint) * 0.15f;
            var p3 = p1 + Vector2.Normalize(diff) * Math.Min(diff.Length(), 40);
            var p4 = p2 + Vector2.Normalize(diff2) * Math.Min(diff2.Length(), 40);

            var roofFace1 = new PolygonG([topPoints[0], topPoints[1], new Point(p3)]);
            var roofFace2 = new PolygonG([topPoints[2], topPoints[3], new Point(p4)]);
            var roofSide1 = new PolygonG([topPoints[1], topPoints[2], new Point(p4), new Point(p3)]);
            var roofSide2 = new PolygonG([topPoints[3], topPoints[0], new Point(p3), new Point(p4)]);

            vRoof[0].UpdatePoly(roofFace1);
            vRoof[0].SetZIndex(baseZindex);
            vRoof[1].UpdatePoly(roofFace2);
            vRoof[1].SetZIndex(baseZindex);
            vRoof[2].UpdatePoly(roofSide1);
            vRoof[2].SetZIndex(baseZindex);
            vRoof[3].UpdatePoly(roofSide2);
            vRoof[3].SetZIndex(baseZindex);



        }

        public void Draw(Vector2 viewPoint)
        {
            var topPoints = vBase._poly.Points.Select(p => new Point(Vector2.Add(p.coord, Vector2.Multiply(Vector2.Subtract(p.coord, viewPoint), this.heightCoef)))).ToList();

            PolygonG ceiling = new PolygonG(topPoints);
            //vCeiling = new VisualPolyGon(this.canvas, ceiling);

            for (int i = 0; i < vBase._poly.Points.Count; i++)
            {
                int nextI = (i+1) % vBase._poly.Points.Count;
                PolygonG sidePoly = new PolygonG([vBase._poly.Points[i], vBase._poly.Points[nextI], topPoints[nextI], topPoints[i]]);
                vSides.Add(new VisualPolyGon(canvas, sidePoly));
            }

            vSides.Sort((a, b) => a._poly.DistanceToPoint(new Point(viewPoint)).CompareTo(b._poly.DistanceToPoint(new Point(viewPoint))));

            //Roof
            var p1 = Vector2.Lerp(topPoints[0].coord, topPoints[1].coord, 0.5f);
            var p2 = Vector2.Lerp(topPoints[2].coord, topPoints[3].coord, 0.5f);
            var diff = (p1 - viewPoint) * 0.15f;
            var diff2 = (p2 - viewPoint) * 0.15f;
            var p3 = p1 + Vector2.Normalize(diff) * Math.Min(diff.Length(), 40);
            var p4 = p2 + Vector2.Normalize(diff2) * Math.Min(diff2.Length(), 40);

            var roofFace1 = new PolygonG([topPoints[0], topPoints[1], new Point(p3)]);
            var roofFace2 = new PolygonG([topPoints[2], topPoints[3], new Point(p4)]);
            var roofSide1 = new PolygonG([topPoints[1], topPoints[2], new Point(p4), new Point(p3)]); 
            var roofSide2 = new PolygonG([topPoints[3], topPoints[0], new Point(p3), new Point(p4)]);
            vRoof.Add(new VisualPolyGon(canvas, roofFace1));
            vRoof.Add(new VisualPolyGon(canvas, roofFace2));
            vRoof.Add(new VisualPolyGon(canvas, roofSide1));
            vRoof.Add(new VisualPolyGon(canvas, roofSide2));

            vBase.Draw(1, BrushesUtils.Gray, BrushesUtils.White);
            vSides.ForEach(s=> s.Draw(1, BrushesUtils.Gray, BrushesUtils.White));
            //vCeiling.Draw(1, BrushesUtils.Gray, BrushesUtils.White);

            for (int i =0; i< vRoof.Count; i++)
            {
                if (i < 2)
                    vRoof[i].Draw(2, BrushesUtils.White, BrushesUtils.White);
                else
                    vRoof[i].Draw(2, BrushesUtils.White, BrushesUtils.Roof);
            }
        }

        public void UnDraw()
        {
            vRoof.ForEach(r => r.UnDraw());
            vBase.UnDraw();
            vSides.ForEach(s => s.UnDraw());
            //vCeiling.UnDraw();
        }

        public PolygonG GetBase() => this.buildingBase;
    }
}
