using System.Numerics;

namespace selfdrivingcar.src
{
    internal class Point
    {
        public Vector2 coord; 

        public Point(float x, float y)
        {
            this.coord = new Vector2(x, y);
        }

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
