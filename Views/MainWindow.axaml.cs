using Avalonia.Controls;
using IRis.Models.Circuit;
using IRis.Services;
using IRis.ViewModels;

namespace IRis.Views;

public partial class MainWindow : Window
{
    public MainWindow(CanvasService canvasService)
    {
        InitializeComponent();
        // "MainCanvas" from the XAML is used for all the drawing
        canvasService.RegisterCanvas(MainCanvas);
        
        
        // DataContext = MainWindowViewModel();
        
        // CircuitComponent component = new CircuitComponent();
        //     
        // MainCanvas.Children.Add(component);
    }
}