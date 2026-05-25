using Avalonia.Controls;
using IRis.ViewModels;


namespace IRis.Views;


public partial class AboutWindow : Window
{
    public AboutWindow()
    {
        InitializeComponent();
        DataContext = new AboutWindowViewModel();
    }
}
