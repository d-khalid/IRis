using Avalonia.Controls;
using IRis.Models;


namespace IRis.Views;


public partial class MainWindow : Window
{
    public MainWindow(Simulation simulation)
    {
        InitializeComponent();
        simulation.Register(MainCanvas);
    }
}

