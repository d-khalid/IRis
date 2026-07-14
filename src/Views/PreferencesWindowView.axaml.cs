using Avalonia.Controls;
using IRis.ViewModels;

namespace IRis.Views;

public partial class PreferencesWindowView : Window
{
    public PreferencesWindowView()
    {
        InitializeComponent();
        DataContext = new PreferencesWindowViewModel();
    }
}
