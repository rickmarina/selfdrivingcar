using selfdrivingcar.src;
using selfdrivingcar.src.visual;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
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

    private VisualGraph? _visualGraph;
    private ViewPort? _viewPort; 

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
        if (_viewPort is not null)
        {
            scaleTransform.ScaleX = 1 / _viewPort.Zoom;
            scaleTransform.ScaleY = 1 / _viewPort.Zoom;
        }
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        _viewPort = new ViewPort(MapCanvas);
        Graph _graph = new Graph([], []);
        _visualGraph = new VisualGraph(_viewPort, _graph);

        _viewPort.ZoomChanged += _viewPort_ZoomChanged;
        _viewPort.OffsetChanged += _viewPort_OffsetChanged;

        _visualGraph.Draw();

        _viewPort.SetOffset(new System.Numerics.Vector2((float)-MapCanvas.ActualWidth / 2, (float)-MapCanvas.ActualHeight / 2));

        translateTransform.X = -MapCanvas.ActualWidth/2;
        translateTransform.Y = -MapCanvas.ActualHeight / 2;

    }

    private void Dispose_Click(object sender, RoutedEventArgs e)
    {
        _visualGraph?.Dispose();
    }
    private void Save_Click(object sender, RoutedEventArgs e)
    {
        var options = new JsonSerializerOptions
        {
            IncludeFields = true  // Incluye los campos
        };

        string json = string.Empty;
        if (_visualGraph != null) 
            json = JsonSerializer.Serialize(_visualGraph, options);

        File.WriteAllText(Path.GetDirectoryName(Environment.ProcessPath) + @"\data.json", json);

    }
    private void Load_Click(object sender, RoutedEventArgs e)
    {

        string json = File.ReadAllText(Path.GetDirectoryName(Environment.ProcessPath) + @"\data.json");

        RootJson? graphJson = JsonSerializer.Deserialize<RootJson>(json);

        if (graphJson is not null) { 
            _visualGraph?.Dispose();
            _visualGraph?.Load(graphJson);
        }


    }
    private void MapCanvas_Loaded(object sender, RoutedEventArgs e)
    {
        Debug.WriteLine($"canvas loaded [{MapCanvas.ActualWidth}x{MapCanvas.ActualHeight}");

    }
}