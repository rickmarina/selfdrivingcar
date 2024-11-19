using System.Numerics;

namespace selfdrivingcar.src
{
    internal class Point
    {
        public Vector2 coord { get; set; } 

        public Point(float x, float y)
        {
            this.coord = new Vector2(x, y);
        }

        public Point(Vector2 v)
        {
            this.coord = v;
        }

        public System.Windows.Point ToWindowsPoint() => new System.Windows.Point(coord.X, coord.Y);

        public override bool Equals(object? obj)
        {
            if (obj is Point otherPoint)
            {
                return coord == otherPoint.coord;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return coord.GetHashCode();
        }
        
    }
}
