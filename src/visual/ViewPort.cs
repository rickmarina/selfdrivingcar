using System.Diagnostics;
using System.Numerics;
using System.Windows.Controls;
using System.Windows.Input;

namespace selfdrivingcar.src.visual
{
    internal class ViewPort
    {
        public Vector2 WindowSize { get; set; }
        public Canvas _canvas { get; private set; }
        public float _zoom = 1;
        public event EventHandler<float>? ZoomChanged;
        public event EventHandler<Vector2>? OffsetChanged;
        public Vector2 Center { get; private set; }
        private DragModel _dragModel;
        public Vector2 Offset { get; private set; } = new Vector2(0,0);
        public float Zoom
        {
            get => _zoom;
            private set
            {
                if (_zoom != value)
                {
                    _zoom = value;
                    OnZoomChanged(_zoom);
                }
            }
        }

        public ViewPort(Canvas canvas, double innerWidth, double innerHeight)
        {
            WindowSize = new Vector2((float)innerWidth, (float)innerHeight);
            _canvas = canvas;
            _canvas.MouseWheel += HandleMouseWheel;
            _canvas.MouseDown += _canvas_MouseDown;
            _canvas.MouseMove += _canvas_MouseMove;
            _canvas.MouseUp += _canvas_MouseUp;
            _dragModel = new DragModel();
            Center = new Vector2((float)_canvas.ActualWidth/2,(float)_canvas.ActualHeight/2);
        }

        public Vector2 GetViewPoint() => (-1*Offset)+(WindowSize*Zoom*0.5f);
        //public Vector2 GetViewPoint() => Vector2.Multiply(Offset, -1);
        public Vector2 GetOffset() => Vector2.Add(Offset, _dragModel.Offset);
        public void SetOffset(Vector2 offset) { this.Offset = offset; }

        private void _canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_dragModel.Active)
            {

                //Offset = Vector2.Add(Offset, _dragModel.Offset);
                //Debug.WriteLine($"offset final {Offset}");
                _dragModel.Reset();
            }
        }

        private void _canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragModel.Active)
            {
                var mouse = e.GetPosition(_canvas);
                _dragModel.End = new Vector2((float)mouse.X, (float)mouse.Y);
                _dragModel.Offset = Vector2.Subtract(_dragModel.End, _dragModel.Start);
                Offset = Vector2.Add(Offset, _dragModel.Offset); //update offset here before we translate canvas and mouseposition resets to origin (start)
                                                                 //add the real offset 

                if (OffsetChanged != null) 
                    OffsetChanged(this, _dragModel.Offset/Zoom); //emit the offset divided by zoom factor
            }
        }

        private void _canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                var mouse = e.GetPosition(_canvas);
                //Debug.WriteLine($"mouse click: {mouse.X}-{mouse.Y}");
                _dragModel.Start = new Vector2((float)mouse.X, (float)mouse.Y);
                _dragModel.Active = true; 
            }
        }

        public System.Windows.Point GetMouse(MouseEventArgs e)
        {
            var mousePosition = e.GetPosition(_canvas);
            return mousePosition;
        }

        private void HandleMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var direction = -1 * Math.Sign(e.Delta);
            float step = 0.1f;
            Zoom += direction * step; 
            Zoom = Math.Clamp(_zoom, 1, 2);

            //var mouse = GetMouse(e);
            //double offsetX = mouse.X - (mouse.X * Zoom);
            //double offsetY = mouse.Y - (mouse.Y * Zoom);
            //_dragModel.Offset = new Vector2((float)offsetX, (float)offsetY);

            //if (OffsetChanged != null)
            //    OffsetChanged(this, Offset);
        }

        protected virtual void OnZoomChanged(float newZoom)
        {
            if (ZoomChanged != null) 
                ZoomChanged(this, newZoom);
            //ZoomChanged?.Invoke(this, newZoom);
        }


    }
}
