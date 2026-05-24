using Avalonia.Controls;
using IRis.ViewModels;


namespace IRis.Views;


public partial class MainWindowView : Window
{
    public MainWindowView()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
}
