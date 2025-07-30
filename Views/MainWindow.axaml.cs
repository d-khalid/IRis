using Avalonia.Controls;
using Avalonia.Input;
using IRis.Models;
using IRis.ViewModels;

namespace IRis.Views;

public partial class MainWindow : Window
{
    public MainWindow() : this(new Simulation()) {} // fallback for preview/designer/runtime

    public MainWindow(Simulation simulation)
    {
        InitializeComponent();
        // "MainCanvas" from the XAML is used for all the drawing
        simulation.Register(MainCanvas);
        
        
        // DataContext = MainWindowViewModel();
        
        // CircuitComponent component = new CircuitComponent();
        //     
        // MainCanvas.Children.Add(component);
    }
}