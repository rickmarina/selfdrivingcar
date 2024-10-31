using System.Diagnostics;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace selfdrivingcar.src
{
    internal class ViewPort
    {

        private readonly Canvas _canvas;
        public float zoom { get; private set; } = 1; 

        public ViewPort(Canvas canvas)
        {
            this._canvas = canvas;

            this._canvas.MouseWheel += HandleMouseWheel;

        }

        public System.Windows.Point GetMouse(MouseEventArgs e)
        {
            var mousePosition = e.GetPosition(this._canvas);
            //mousePosition.X * this.zoom;
            //mousePosition.Y * this.zoom;
            return new System.Windows.Point();
        }

        private void HandleMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var direction = -1*Math.Sign(e.Delta);
            float step = 0.1f; 
            this.zoom += direction * step;
            this.zoom = Math.Clamp(this.zoom, 1, 5);
            Debug.WriteLine(this.zoom);
        }


    }
}
