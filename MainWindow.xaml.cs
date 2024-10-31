using selfdrivingcar.src;
using selfdrivingcar.src.visual;
using System.Diagnostics;
using System.Windows;
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
        _viewPort = new ViewPort(MapCanvas);
        Point? p1 = new Point(200, 200);
        Point p2 = new Point(500, 200);
        Point p3 = new Point(400, 400);
        Point p4 = new Point(100, 300);

        Segment s1 = new Segment(p1, p2);
        Segment s2 = new Segment(p1, p3);
        Segment s3 = new Segment(p1, p4);
        Segment s4 = new Segment(p2, p3);

        Graph _graph = new Graph([p1, p2, p3, p4], [s1, s2, s3, s4]);

        _visualGraph = new VisualGraph(MapCanvas, _graph);
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        _visualGraph.Draw();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        int idx = Random.Shared.Next(_visualGraph.VisualPoints.Count);
        _visualGraph.RemovePoint(_visualGraph.VisualPoints[idx].GetPoint());
            
    }
}