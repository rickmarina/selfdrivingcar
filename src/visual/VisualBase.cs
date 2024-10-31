using System.Windows.Controls;
using System.Windows.Shapes;

namespace selfdrivingcar.src.visual
{
    internal abstract class VisualBase<T> where T : Shape
    {
        private bool addedToCanvas = false;
        protected readonly Canvas _canvas;
        protected T? shape;

        protected VisualBase(Canvas canvas)
        {
            _canvas = canvas;
        }

        public void AddToCanvas()
        {
            if (addedToCanvas)
                throw new InvalidOperationException("Point is already added to canvas");

            addedToCanvas = true;
            _canvas.Children.Add(shape);
        }
        public void RemoveFromCanvas()
        {
            if (!addedToCanvas)
                throw new InvalidOperationException("Point is already removed from canvas");

            addedToCanvas = false;
            _canvas.Children.Remove(shape);
        }
    }
}
