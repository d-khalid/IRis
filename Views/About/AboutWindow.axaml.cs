// AboutWindow.axaml.cs

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace IRis.Views;

public partial class AboutWindow : Window
{
    public AboutWindow()
    {
        InitializeComponent();
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}