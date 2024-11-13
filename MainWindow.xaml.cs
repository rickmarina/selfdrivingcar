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
    private World? world;

    public MainWindow()
    {
        InitializeComponent();
    }


    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        
        world = new World(mainWindow, MapCanvas, new src.world.WorldSettings(), scaleTransform, translateTransform);
    }

    private void Dispose_Click(object sender, RoutedEventArgs e)
    {
        //_visualGraph?.Dispose();
    }
    private void Save_Click(object sender, RoutedEventArgs e)
    {
        var options = new JsonSerializerOptions
        {
            IncludeFields = true  // Incluye los campos
        };

        string json = string.Empty;
        var vgraph = world?.GetVisualGraph();
        json = JsonSerializer.Serialize(vgraph, options);

        File.WriteAllText(Path.GetDirectoryName(Environment.ProcessPath) + @"\data.json", json);

    }
    private void Load_Click(object sender, RoutedEventArgs e)
    {

        string json = File.ReadAllText(Path.GetDirectoryName(Environment.ProcessPath) + @"\data.json");

        RootJson? graphJson = JsonSerializer.Deserialize<RootJson>(json);

        world?.Dispose();
        world?.Load(graphJson, true);

    }
    private void MapCanvas_Loaded(object sender, RoutedEventArgs e)
    {
        Debug.WriteLine($"Canvas loaded");

    }
}