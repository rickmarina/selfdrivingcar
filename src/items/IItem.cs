using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace selfdrivingcar.src.items
{
    internal interface IItem
    {
        PolygonG GetBase();
        void UpdateViewPoint(Vector2 viewPoint, int? zindex);
    }
}
