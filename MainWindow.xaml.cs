using selfdrivingcar.src;
using selfdrivingcar.src.visual;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Point = selfdrivingcar.src.Point;

namespace selfdrivingcar;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private System.Windows.Point lastMousePosition;
    private bool isDragging;
    private double TotalZoom = 0;
    private double MaxZoom = 5;
    private double MinZoom = -5;

    private VisualGraph _visualGraph;
    private ViewPort _viewPort; 

    public MainWindow()
    {
        InitializeComponent();
    }

    private void _viewPort_OffsetChanged(object? sender, System.Numerics.Vector2 e)
    {
        translateTransform.X += e.X;
        translateTransform.Y += e.Y;
    }

    private void _viewPort_ZoomChanged(object? sender, float e)
    {
        scaleTransform.ScaleX = 1 / _viewPort.Zoom;
        scaleTransform.ScaleY = 1 / _viewPort.Zoom;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        Point? p1 = new Point(200, 200);
        Point p2 = new Point(500, 200);
        Point p3 = new Point(400, 400);
        Point p4 = new Point(100, 300);

        Segment s1 = new Segment(p1, p2);
        Segment s2 = new Segment(p1, p3);
        Segment s3 = new Segment(p1, p4);
        Segment s4 = new Segment(p2, p3);

        _viewPort = new ViewPort(MapCanvas);
        Graph _graph = new Graph([p1, p2, p3, p4], [s1, s2, s3, s4]);
        _visualGraph = new VisualGraph(_viewPort, _graph);

        _viewPort.ZoomChanged += _viewPort_ZoomChanged;
        _viewPort.OffsetChanged += _viewPort_OffsetChanged;

        _visualGraph.Draw();
        Debug.WriteLine($"canvas [{MapCanvas.ActualWidth}x{MapCanvas.ActualHeight}");

        _viewPort.SetOffset(new System.Numerics.Vector2((float)-MapCanvas.ActualWidth / 2, (float)-MapCanvas.ActualHeight / 2));

        translateTransform.X = -MapCanvas.ActualWidth/2;
        translateTransform.Y = -MapCanvas.ActualHeight / 2;

    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {

    }

    private void MapCanvas_Loaded(object sender, RoutedEventArgs e)
    {
        Debug.WriteLine($"canvas loaded [{MapCanvas.ActualWidth}x{MapCanvas.ActualHeight}");

    }
}