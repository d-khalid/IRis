// MainWindow.axaml.cs
using Avalonia.Controls;
using IRis.Models;

namespace IRis.Views;

public partial class MainWindow : Window
{
    // This parameter-less constructor is not run from app.axaml.cs
    // Otherwise it could cause errors
    public MainWindow() : this(new Simulation()) {} // fallback for preview/designer/runtime

    public MainWindow(Simulation simulation)
    {
        InitializeComponent();

        // "MainCanvas" from the XAML is used for all the drawing
        // Register this to the simulation object
        simulation.Register(MainCanvas);
    }
}
