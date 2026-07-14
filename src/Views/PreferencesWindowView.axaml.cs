using Avalonia.Controls;
using IRis.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace IRis.Views;

public partial class PreferencesWindowView : Window
{
    public PreferencesWindowView()
    {
        InitializeComponent();
        DataContext = App.Current.Services.GetRequiredService<PreferencesWindowViewModel>();
    }
}
