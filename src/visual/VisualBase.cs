using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Shapes;
using static selfdrivingcar.src.world.Enums;

namespace selfdrivingcar.src.visual
{
    internal abstract class VisualBase<T> where T : Shape
    {
        public bool addedToCanvas { get; private set; } = false;
        protected readonly Canvas _canvas;
        protected T? shape;

        protected VisualBase(Canvas canvas)
        {
            _canvas = canvas;
        }

        public void AddToCanvas(ZINDEXES zindex)
        {
            if (addedToCanvas)
                throw new InvalidOperationException("Point is already added to canvas");

            addedToCanvas = true;
            Canvas.SetZIndex(shape, Convert.ToInt32(zindex));
            _canvas.Children.Add(shape);
        }
        public void RemoveFromCanvas()
        {
            if (!addedToCanvas)
                Debug.WriteLine("Point is already removed from canvas");

            addedToCanvas = false;
            _canvas.Children.Remove(shape);
        }
    }
}
