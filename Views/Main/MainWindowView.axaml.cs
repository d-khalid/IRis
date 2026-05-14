using System;
using Avalonia.Controls;


namespace IRis.Views.Main;


public partial class MainWindowView : Window
{
    public MainWindowView()
    {
        Avalonia.Markup.Xaml.AvaloniaXamlLoader.Load(this);
    }
}
