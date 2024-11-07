using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace selfdrivingcar.src.visual
{
    internal class DragModel
    {
        public Vector2 Start { get; set; } = new Vector2(0, 0);
        public Vector2 End { get; set; } = new Vector2(0, 0);
        public Vector2 Offset { get; set; } = new Vector2(0, 0);
        public bool Active { get; set; } = false;

        public void Reset()
        {
            Start = new Vector2(0, 0); End = new Vector2(0, 0); Offset = new Vector2(0, 0); Active = false;
        }
    }
}
